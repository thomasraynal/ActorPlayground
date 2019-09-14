using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreAdapterFactory : IQueueAdapterFactory
    {
        private readonly IEventStoreRepositoryConfiguration _eventStoreRepositoryConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IStreamQueueMapper _streamQueueMapper;
        private readonly string _providerName;
        private readonly EventStoreQueueAdapterCache _eventStoreQueueAdapterCache;
        private readonly ConcurrentDictionary<QueueId, EventStoreQueueAdapterReceiver> _receivers;

        public static EventStoreAdapterFactory Create(IServiceProvider services, string name)
        {
            var repositoryConfiguration = services.GetOptionsByName<EventStoreRepositoryConfiguration>(name);

            return ActivatorUtilities.CreateInstance<EventStoreAdapterFactory>(services, name, repositoryConfiguration);
        }

        public EventStoreAdapterFactory(string providerName, IEventStoreRepositoryConfiguration repositoryConfiguration, ILoggerFactory loggerFactory)
        {
            _eventStoreRepositoryConfiguration = repositoryConfiguration;
            _loggerFactory = loggerFactory;
            _providerName = providerName;

            _receivers = new ConcurrentDictionary<QueueId, EventStoreQueueAdapterReceiver>();

            _eventStoreQueueAdapterCache = new EventStoreQueueAdapterCache(this, loggerFactory);

            var hashRingStreamQueueMapperOptions = new HashRingStreamQueueMapperOptions() { TotalQueueCount = 1 };
            _streamQueueMapper = new HashRingBasedStreamQueueMapper(hashRingStreamQueueMapperOptions, _providerName);

        }

        public Task<IQueueAdapter> CreateAdapter()
        {
            var adpter = new EventStoreQueueAdapter(_providerName, _eventStoreRepositoryConfiguration, _loggerFactory);
            return Task.FromResult<IQueueAdapter>(adpter);
        }

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return _receivers.GetOrAdd(queueId, ConstructReceiver);
        }

        private EventStoreQueueAdapterReceiver ConstructReceiver(QueueId queueId)
        {
            return (EventStoreQueueAdapterReceiver)EventStoreQueueAdapterReceiver.Create(_eventStoreRepositoryConfiguration, _loggerFactory, queueId, _providerName);
        }

        public Task<IStreamFailureHandler> GetDeliveryFailureHandler(QueueId queueId)
        {
            return Task.FromResult<IStreamFailureHandler>(new NoOpStreamDeliveryFailureHandler(false));
        }

        public IQueueAdapterCache GetQueueAdapterCache()
        {
            return _eventStoreQueueAdapterCache;
        }

        public IStreamQueueMapper GetStreamQueueMapper()
        {
            return _streamQueueMapper;
        }
    }
}
