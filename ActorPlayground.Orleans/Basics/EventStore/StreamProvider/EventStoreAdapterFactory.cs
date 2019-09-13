using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreAdapterFactory : IQueueAdapterFactory
    {
        private readonly IEventStoreRepositoryConfiguration _repositoryConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IStreamQueueMapper _streamQueueMapper;
        private readonly string _providerName;
        private readonly SimpleQueueAdapterCache _eventStoreQueueAdapterCache;

        public static EventStoreAdapterFactory Create(IServiceProvider services, string name)
        {
            var repositoryConfiguration = services.GetOptionsByName<EventStoreRepositoryConfiguration>(name);
            var cacheOptions = services.GetOptionsByName<SimpleQueueCacheOptions>(name);

            return ActivatorUtilities.CreateInstance<EventStoreAdapterFactory>(
                services,
                name,
                repositoryConfiguration,
                cacheOptions);
        }

        public EventStoreAdapterFactory(string providerName, 
            IEventStoreRepositoryConfiguration repositoryConfiguration, 
            SimpleQueueCacheOptions simpleQueueCacheOptions, 
            ILoggerFactory loggerFactory)
        {
            _repositoryConfiguration = repositoryConfiguration;
            _loggerFactory = loggerFactory;
            _providerName = providerName;

            _eventStoreQueueAdapterCache = new SimpleQueueAdapterCache(simpleQueueCacheOptions, providerName, loggerFactory);

            var hashRingStreamQueueMapperOptions = new HashRingStreamQueueMapperOptions() { TotalQueueCount = 1 };
            _streamQueueMapper = new HashRingBasedStreamQueueMapper(hashRingStreamQueueMapperOptions, _providerName);

        }

        public Task<IQueueAdapter> CreateAdapter()
        {
            var adpter = new EventStoreQueueAdapter(_providerName, _repositoryConfiguration, _loggerFactory);
            return Task.FromResult<IQueueAdapter>(adpter);
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
