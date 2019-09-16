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
        private readonly IEventStoreRepositoryConfiguration _streamProviderConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IStreamQueueMapper _streamQueueMapper;
        private readonly string _providerName;
        private readonly EventStoreQueueAdapterCache _eventStoreQueueAdapterCache;
        private readonly ConcurrentDictionary<QueueId, EventStoreQueueAdapterReceiver> _receivers;

        public static EventStoreAdapterFactory Create(IServiceProvider services, string name)
        {
            var streamProviderConfiguration = services.GetOptionsByName<EventStoreStreamProviderConfiguration>(name);

            return ActivatorUtilities.CreateInstance<EventStoreAdapterFactory>(services, name, streamProviderConfiguration);
        }

        public EventStoreAdapterFactory(string providerName, IEventStoreStreamProviderConfiguration streamProviderConfiguration, ILoggerFactory loggerFactory)
        {
            _streamProviderConfiguration = streamProviderConfiguration;
            _loggerFactory = loggerFactory;
            _providerName = providerName;

            _receivers = new ConcurrentDictionary<QueueId, EventStoreQueueAdapterReceiver>();

            _eventStoreQueueAdapterCache = new EventStoreQueueAdapterCache(this, loggerFactory);

            var hashRingStreamQueueMapperOptions = new HashRingStreamQueueMapperOptions() { TotalQueueCount = 2 };
            _streamQueueMapper = new HashRingBasedStreamQueueMapper(hashRingStreamQueueMapperOptions, _providerName);

        }

        public Task<IQueueAdapter> CreateAdapter()
        {
            var adapter = new EventStoreQueueAdapter(_providerName, _streamProviderConfiguration, _loggerFactory);
            return Task.FromResult<IQueueAdapter>(adapter);
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
