using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Providers.Streams.Common;
using Orleans.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreStreamProvider : PersistentStreamProvider
    {
        public EventStoreStreamProvider(string name, StreamPubSubOptions pubsubOptions, StreamLifecycleOptions lifeCycleOptions, IProviderRuntime runtime, SerializationManager serializationManager, ILogger<PersistentStreamProvider> logger) : base(name, pubsubOptions, lifeCycleOptions, runtime, serializationManager, logger)
        {
        }

    }
}
