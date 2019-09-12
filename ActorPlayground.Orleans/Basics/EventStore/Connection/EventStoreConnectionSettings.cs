using System;
using EventStore.ClientAPI;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public static class EventStoreConnectionSettings
    {
        public static readonly ConnectionSettings Default = ConnectionSettings.Create().KeepRetrying().KeepReconnecting().Build();
    }
}