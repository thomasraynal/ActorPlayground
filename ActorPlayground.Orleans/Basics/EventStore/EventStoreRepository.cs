using EventStore.ClientAPI;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreRepository
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly IEventStoreRepositoryConfiguration _configuration;
        private readonly Dictionary<string, Type> _eventTypeCache;
        private readonly IDisposable _cleanup;

        private bool _isConnected;

        public EventStoreRepository(IEventStoreRepositoryConfiguration configuration, IEventStoreConnection eventStoreConnection, IConnectionStatusMonitor connectionMonitor)
        {

            _configuration = configuration;
            _eventStoreConnection = eventStoreConnection;
            _eventTypeCache = new Dictionary<string, Type>();

            _cleanup = connectionMonitor
                        .IsConnected
                        .Subscribe(obs => _isConnected = obs);

        }

        public async Task<(int, TState)> GetById<TKey, TState>(TKey id) where TState : class, IAggregate, new()
        {
            if (!_isConnected) throw new InvalidOperationException("not connected");

            var streamName = $"{id}";

            var eventNumber = 0L;

            var aggregate = new TState();

            var events = new List<KeyValuePair<int, TState>>();

            StreamEventsSlice currentSlice;

            do
            {
                currentSlice = await _eventStoreConnection.ReadStreamEventsForwardAsync(streamName, eventNumber, _configuration.ReadPageSize, false);

                if (currentSlice.Status == SliceReadStatus.StreamNotFound || currentSlice.Status == SliceReadStatus.StreamDeleted)
                {
                    break;
                }

                eventNumber = currentSlice.NextEventNumber;

                foreach (var resolvedEvent in currentSlice.Events)
                {
                    var @event = DeserializeEvent(resolvedEvent.Event);

                    aggregate.Apply(@event);

                }

            } while (!currentSlice.IsEndOfStream);


            return ((int)eventNumber, aggregate);
        }

        private Type GetEventType(string type)
        {
            if (!_eventTypeCache.ContainsKey(type))
            {
                _eventTypeCache[type] = Type.GetType(type, true, true);
            }

            return _eventTypeCache[type];
        }

        public async Task Save<TAggregate>(string streamId, int originalVersion, IEnumerable<IEvent> pendingEvents, params KeyValuePair<string, string>[] extraHeaders) where TAggregate : IAggregate, new()
        {

            WriteResult result;

            var commitHeaders = CreateCommitHeaders<TAggregate>(extraHeaders);
            var eventsToSave = pendingEvents.Select(ev => ToEventData(ev, commitHeaders));

            var eventBatches = GetEventBatches(eventsToSave);

            if (eventBatches.Count == 1)
            {
                result = await _eventStoreConnection.AppendToStreamAsync(streamId, originalVersion, eventBatches[0]);
            }
            else
            {
                using (var transaction = await _eventStoreConnection.StartTransactionAsync(streamId, originalVersion))
                {
                    foreach (var batch in eventBatches)
                    {
                        await transaction.WriteAsync(batch);
                    }

                    result = await transaction.CommitAsync();
                }
            }

        }

        private IEvent DeserializeEvent(RecordedEvent evt)
        {
            return _configuration.Serializer.DeserializeObject(evt.Data, GetEventType(evt.EventType)) as IEvent;
        }

        private IList<IList<EventData>> GetEventBatches(IEnumerable<EventData> events)
        {
            return events.Batch(_configuration.WritePageSize).Select(x => (IList<EventData>)x.ToList()).ToList();
        }

        protected virtual IDictionary<string, string> GetCommitHeaders<TAggregate>()
        {
            var commitId = Guid.NewGuid();

            return new Dictionary<string, string>
            {
                {MetadataKeys.CommitIdHeader, commitId.ToString()},
                {MetadataKeys.AggregateClrTypeHeader, typeof(TAggregate).AssemblyQualifiedName},
                {MetadataKeys.UserIdentityHeader, Thread.CurrentPrincipal?.Identity?.Name},
                {MetadataKeys.ServerNameHeader, Environment.MachineName},
                {MetadataKeys.ServerClockHeader, DateTime.UtcNow.ToString("o")}
            };
        }

        private IDictionary<string, string> CreateCommitHeaders<TAggregate>(KeyValuePair<string, string>[] extraHeaders)
        {
            var commitHeaders = GetCommitHeaders<TAggregate>();

            foreach (var extraHeader in extraHeaders)
            {
                commitHeaders[extraHeader.Key] = extraHeader.Value;
            }

            return commitHeaders;
        }

        private EventData ToEventData(IEvent @event, IDictionary<string, string> headers)
        {
            var eventId = Guid.NewGuid();
            var data = _configuration.Serializer.SerializeObject(@event);

            var eventHeaders = new Dictionary<string, string>(headers)
            {
                {MetadataKeys.EventClrTypeHeader, @event.GetType().AssemblyQualifiedName}
            };

            var metadata = _configuration.Serializer.SerializeObject(eventHeaders);
            var typeName = @event.GetType().ToString();

            return new EventData(eventId, typeName, true, data, metadata);
        }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}
