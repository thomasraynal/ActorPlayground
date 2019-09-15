using EventStore.ClientAPI.SystemData;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IEventStorePersistentSubscriptionConfiguration
    {
        UserCredentials UserCredentials { get; }
        int BufferSize { get; }
        bool AutoAck { get; }
    }
}
