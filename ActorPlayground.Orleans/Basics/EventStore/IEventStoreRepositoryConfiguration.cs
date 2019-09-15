using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IEventStoreRepositoryConfiguration : IEventStorePersistentSubscriptionConfiguration
    {
        int WritePageSize { get; }
        int ReadPageSize { get; }
        ISerializer Serializer { get; }
        string ConnectionString { get; }
        ConnectionSettings ConnectionSettings { get; }
    }
}
