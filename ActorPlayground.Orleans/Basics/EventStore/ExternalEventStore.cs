using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class ExternalEventStore : IEventStore
    {
        public ExternalEventStore(string url)
        {
            Connection = EventStoreConnection.Create(EventStoreConnectionSettings.Default, new Uri(url));
        }

        public ExternalEventStore(string url, ConnectionSettings settings)
        {
            Connection = EventStoreConnection.Create(settings, new Uri(url));
        }

        public ExternalEventStore(IEventStoreConnection connection)
        {
            Connection = connection;
        }

        public IEventStoreConnection Connection { get; }
    }
}
