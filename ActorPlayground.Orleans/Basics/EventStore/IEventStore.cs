using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IEventStore
    {
        IEventStoreConnection Connection { get; }
    }
}
