using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreRepositoryConfiguration : IEventStoreRepositoryConfiguration
    {
        public int WritePageSize { get; set; } = 500;
        public int ReadPageSize { get; set; } = 500;
        public ISerializer Serializer { get; set; } = new JsonSerializer();
        public string ConnectionString { get; set; } = $"tcp://admin:changeit@localhost:1113";
        public ConnectionSettings ConnectionSettings { get; set; } = ConnectionSettings.Default;
    }
}
