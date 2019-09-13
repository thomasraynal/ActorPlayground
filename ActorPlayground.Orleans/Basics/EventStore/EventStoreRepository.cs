﻿using EventStore.ClientAPI;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly IEventStoreRepositoryConfiguration _configuration;
        private readonly Dictionary<string, Type> _eventTypeCache;
        private readonly IConnectionStatusMonitor _connectionMonitor;

        public bool IsConnected => _connectionMonitor.IsConnected;

        public static EventStoreRepository Create(IEventStoreRepositoryConfiguration repositoryConfiguration)
        {
            var eventStoreConnection = new ExternalEventStore(repositoryConfiguration.ConnectionString, repositoryConfiguration.ConnectionSettings).Connection;
            var repository = new EventStoreRepository(repositoryConfiguration, eventStoreConnection, new ConnectionStatusMonitor(eventStoreConnection));

            return repository;
        }

        private EventStoreRepository(IEventStoreRepositoryConfiguration configuration, IEventStoreConnection eventStoreConnection, IConnectionStatusMonitor connectionMonitor)
        {
            _configuration = configuration;
            _eventStoreConnection = eventStoreConnection;
            _eventTypeCache = new Dictionary<string, Type>();
            _connectionMonitor = connectionMonitor;
        }

        public async Task Connect(TimeSpan timeout)
        {
            await _connectionMonitor.Connect();
            await Wait.Until(() => IsConnected, timeout);
        }

        public IObservable<IEvent> Observe(string streamId, long? fromIncluding = null, bool rewindAfterDisconnection = false)
        {
            return CreateSubscription(streamId, fromIncluding, rewindAfterDisconnection)
                 .TakeUntil((_) => !IsConnected)
                 .Retry();
        }

        public async Task<(int version, TState aggregate)> GetAggregate<TKey, TState>(TKey id) where TState : class, IAggregate, new()
        {
            if (!IsConnected) throw new InvalidOperationException("not connected");

            var streamName = $"{id}";

            var eventNumber = 0L;

            var aggregate = new TState();

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

        public async Task SavePendingEvents(string streamId, long originalVersion, IEnumerable<IEvent> pendingEvents, params KeyValuePair<string, string>[] extraHeaders)
        {

            if (!IsConnected) throw new InvalidOperationException("not connected");

            WriteResult result;

            var commitHeaders = CreateCommitHeaders(extraHeaders);
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

        private IObservable<IEvent> CreateSubscription(string streamId, long? fromIncluding, bool rewindfAfterDisconnection)
        {
            return Observable.Create<IEvent>(async (obs) =>
            {
                return  await CreateEventStoreSubscription(streamId, obs, fromIncluding, rewindfAfterDisconnection);
            });
        }

        private async Task<IDisposable> CreateEventStoreSubscription(string streamId, IObserver<IEvent> obs, long? fromIncluding, bool rewindfAfterDisconnection)
        {
            if (fromIncluding != null || rewindfAfterDisconnection)
            {
                var settings = new CatchUpSubscriptionSettings(10000, 500, false, true, streamId);

                //SubscribeToStreamFrom lastChekcpoint is exclusive
                long? position = fromIncluding == null ? null : fromIncluding - 1;

                var catchUpSubscription = _eventStoreConnection.SubscribeToStreamFrom(streamId, position, settings, (eventStoreSubscription, resolvedEvent) =>
                {
                    var @event = DeserializeEvent(resolvedEvent.Event);

                    obs.OnNext(@event);

                }, (eventStoreCatchUpSubscription) => { }
                 , (subscription, reason, exception) =>
                 {
                     if(null != exception) obs.OnError(exception);
                     else
                     {
                         obs.OnError(new Exception($"{reason}"));
                     }
                 });

                return Disposable.Create(() =>
                {
                    catchUpSubscription.Stop();
                });

            }
            else
            {
                var eventStoreSubscription = await _eventStoreConnection.SubscribeToStreamAsync(streamId, true, (subscription, resolvedEvent) =>
                {
                    var @event = DeserializeEvent(resolvedEvent.Event);

                    obs.OnNext(@event);

                }, (subscription, reason, exception) =>
                {
                    if (null != exception) obs.OnError(exception);
                    else
                    {
                        obs.OnError(new Exception($"{reason}"));
                    }
                });

                return Disposable.Create(() =>
                {
                    eventStoreSubscription.Close();
                });
            }

        }

        private Type GetEventType(string type)
        {
            if (!_eventTypeCache.ContainsKey(type))
            {
                _eventTypeCache[type] = Type.GetType(type, true, true);
            }

            return _eventTypeCache[type];
        }

        private IEvent DeserializeEvent(RecordedEvent evt)
        {
            return _configuration.Serializer.DeserializeObject(evt.Data, GetEventType(evt.EventType)) as IEvent;
        }

        private IList<IList<EventData>> GetEventBatches(IEnumerable<EventData> events)
        {
            return events.Batch(_configuration.WritePageSize).Select(x => (IList<EventData>)x.ToList()).ToList();
        }

        private IDictionary<string, string> CreateCommitHeaders(KeyValuePair<string, string>[] extraHeaders)
        {
            var commitHeaders = GetCommitHeaders();

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

        private IDictionary<string, string> GetCommitHeaders()
        {
            var commitId = Guid.NewGuid();

            return new Dictionary<string, string>
            {
                {MetadataKeys.CommitIdHeader, commitId.ToString()},
                {MetadataKeys.UserIdentityHeader, Thread.CurrentPrincipal?.Identity?.Name},
                {MetadataKeys.ServerNameHeader, Environment.MachineName},
                {MetadataKeys.ServerClockHeader, DateTime.UtcNow.ToString("o")}
            };
        }

        public void Dispose()
        {
            _eventStoreConnection.Close();
            _connectionMonitor.Dispose();
        }


    }
}
