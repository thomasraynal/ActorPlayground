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
    public class EventStoreQueueAdapterReceiver : IQueueAdapterReceiver
    {

        private EventStoreRepository _eventStoreRepository;
        private IDisposable _cleanUp;
        private readonly ConcurrentQueue<IBatchContainer> _receivedMessages;
        private readonly QueueId _queueId;

        public EventStoreQueueAdapterReceiver(IEventStoreRepositoryConfiguration eventStoreRepositoryConfiguration, ILoggerFactory loggerFactory, QueueId queueId, string providerName)
        {
            _eventStoreRepository = EventStoreRepository.Create(eventStoreRepositoryConfiguration);
            _receivedMessages = new ConcurrentQueue<IBatchContainer>();
            _queueId = queueId;
        }

        public static IQueueAdapterReceiver Create(IEventStoreRepositoryConfiguration repositoryConfiguration, ILoggerFactory loggerFactory, QueueId queueId, string providerName)
        {
            return new EventStoreQueueAdapterReceiver(repositoryConfiguration, loggerFactory, queueId, providerName);
        }

        public void CreateSubscription(IStreamIdentity streamIdentity, EventSequenceToken token)
        {

            _cleanUp = _eventStoreRepository.Observe(streamIdentity.Namespace, null, true)
                                            .Subscribe(ev =>
                                            {
                                                _receivedMessages.Enqueue(new EventStoreBatchContainer(Guid.Empty, streamIdentity.Namespace, token, ev));
                                            });
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
