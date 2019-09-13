using Microsoft.Extensions.Logging;
using Orleans.Providers.Streams.Common;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreAdapterReceiver : IQueueAdapterReceiver
    {
        private readonly IEventStoreRepositoryConfiguration _repositoryConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly QueueId _queueId;
        private readonly string _providerName;

        private EventStoreRepository _eventStoreRepository;
        private IDisposable _cleanUp;
        private readonly ConcurrentQueue<IBatchContainer> _receivedMessages;

        public EventStoreAdapterReceiver(IEventStoreRepositoryConfiguration eventStoreRepositoryConfiguration, ILoggerFactory loggerFactory, QueueId queueId, string providerName)
        {
            _repositoryConfiguration = eventStoreRepositoryConfiguration;
            _loggerFactory = loggerFactory;
            _queueId = queueId;
            _providerName = providerName;
            _eventStoreRepository = EventStoreRepository.Create(eventStoreRepositoryConfiguration);
            _receivedMessages = new ConcurrentQueue<IBatchContainer>();
        }

        public static IQueueAdapterReceiver Create(IEventStoreRepositoryConfiguration repositoryConfiguration, ILoggerFactory loggerFactory, QueueId queueId, string providerName)
        {
            return new EventStoreAdapterReceiver(repositoryConfiguration, loggerFactory, queueId, providerName);
        }

        public async Task CreateSubscription(string streamId, EventSequenceToken eventSequenceToken)
        {

            await _eventStoreRepository.Connect(TimeSpan.FromSeconds(5));

            long? position = (eventSequenceToken == null || eventSequenceToken.EventIndex == int.MinValue) ? null : (long?)eventSequenceToken.EventIndex;

            _cleanUp = _eventStoreRepository.Observe(streamId, position, true)
                                        .Subscribe(ev => _receivedMessages.Enqueue(new EventStoreBatchContainer(Guid.Empty, streamId, eventSequenceToken, ev)));

        }

        public Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
        {
            IList<IBatchContainer> containerList = new List<IBatchContainer>();

            while (containerList.Count < maxCount && _receivedMessages.TryDequeue(out IBatchContainer container))
            {
                containerList.Add(container);
            }

            return Task.FromResult(containerList);
        }

        public Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
        {
            //todo: handle ack for persistent subscriptions
            return Task.CompletedTask;
        }

        public Task Shutdown(TimeSpan timeout)
        {
            if (null != _cleanUp) _cleanUp.Dispose();
            _eventStoreRepository.Dispose();

            return Task.CompletedTask;
        }

        public async Task Initialize(TimeSpan timeout)
        {
            await _eventStoreRepository.Connect(timeout);
        }
    }
}
