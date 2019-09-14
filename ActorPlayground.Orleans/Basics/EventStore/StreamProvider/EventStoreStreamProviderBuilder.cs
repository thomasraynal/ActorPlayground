using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public static class EventStoreStreamProviderBuilder
    {
        private const int DefaultCacheSize = 4096;

        public static IClientBuilder AddEventStoreStreamProvider(this IClientBuilder builder, string providerName, Action<EventStoreRepositoryConfiguration> configureOptions = null)
        {
            if (null == configureOptions) configureOptions = (_) => { };

            return AddClientProvider(builder, providerName, opt => opt.Configure(configureOptions));
        }

        public static ISiloHostBuilder AddEventStoreStreamProvider(this ISiloHostBuilder builder, string providerName, Action<EventStoreRepositoryConfiguration> configureOptions = null)
        {
            if (null == configureOptions) configureOptions = (_) => { };

            return AddSiloHostProvider(builder, providerName, opt => opt.Configure(configureOptions));
        }

        private static IClientBuilder AddClientProvider(IClientBuilder builder, string providerName, Action<OptionsBuilder<EventStoreRepositoryConfiguration>> configureOptions = null)
        {
            builder
                .ConfigureApplicationParts(parts => parts.AddFrameworkPart(typeof(EventStoreAdapterFactory).Assembly).WithReferences())
                .ConfigureServices(services =>
                {
                    services
                        .ConfigureNamedOptionForLogging<EventStoreRepositoryConfiguration>(providerName)
                        .ConfigureNamedOptionForLogging<HashRingStreamQueueMapperOptions>(providerName);

                })
                .AddPersistentStreams(providerName, EventStoreAdapterFactory.Create, stream => stream.Configure(configureOptions));

            return builder;
        }

        private static ISiloHostBuilder AddSiloHostProvider(this ISiloHostBuilder builder,string providerName, Action<OptionsBuilder<EventStoreRepositoryConfiguration>> configureOptions = null)
        {
            builder
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(EventStoreAdapterFactory).Assembly).WithReferences())
                .ConfigureServices(services =>
                {
                    services
                        .ConfigureNamedOptionForLogging<EventStoreRepositoryConfiguration>(providerName)
                        .ConfigureNamedOptionForLogging<HashRingStreamQueueMapperOptions>(providerName);

                })
                .AddPersistentStreams(providerName, EventStoreAdapterFactory.Create, stream => stream.Configure(configureOptions));

            return builder;
        }
    }
}
