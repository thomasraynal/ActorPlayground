using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreQueueAdapter : IQueueAdapter
    {
        private readonly IEventStoreRepositoryConfiguration _eventStoreRepositoryConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _providerName;
        private readonly EventStoreRepository _eventStoreRepository;

        public EventStoreQueueAdapter(string providerName, IEventStoreRepositoryConfiguration eventStoreRepositoryConfiguration, ILoggerFactory loggerFactory)
        {
            _eventStoreRepositoryConfiguration = eventStoreRepositoryConfiguration;
            _loggerFactory = loggerFactory;
            _providerName = providerName;

            //todo: use 2 repository (queue adpater + receiver)
            //todo: dispose
            _eventStoreRepository = EventStoreRepository.Create(eventStoreRepositoryConfiguration);
        }

        public string Name { get; private set; }

        public bool IsRewindable => true;

        public StreamProviderDirection Direction => StreamProviderDirection.ReadWrite;

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return EventStoreAdapterReceiver.Create(_eventStoreRepositoryConfiguration, _loggerFactory, queueId, Name);
        }

        public async Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            if (!_eventStoreRepository.IStarted)
            {
                await _eventStoreRepository.Connect(TimeSpan.FromSeconds(5));
            }

            //handle failure not connected
            await _eventStoreRepository.SavePendingEvents(streamNamespace, ExpectedVersion.Any, events.Cast<IEvent>());

        }
    }
}
