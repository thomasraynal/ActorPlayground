using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreQueueAdapterCache : IQueueAdapterCache
    {
        private readonly EventStoreAdapterFactory _adapterFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ConcurrentDictionary<QueueId, EventStoreQueueCache> _queueCaches;

        public EventStoreQueueAdapterCache(EventStoreAdapterFactory adapterFactory, ILoggerFactory loggerFactory)
        {
            _adapterFactory = adapterFactory;
            _loggerFactory = loggerFactory;
            _queueCaches = new ConcurrentDictionary<QueueId, EventStoreQueueCache>();
        }

        public IQueueCache CreateQueueCache(QueueId queueId)
        {
            return _queueCaches.GetOrAdd(queueId, (queue) =>
            {
                var receiver = (EventStoreQueueAdapterReceiver)_adapterFactory.CreateReceiver(queue);
                return new EventStoreQueueCache(100, receiver);

            });
        }

    }
}
