using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreRepositoryConfiguration : IEventStoreRepositoryConfiguration
    {
        public static readonly EventStoreRepositoryConfiguration Default = new EventStoreRepositoryConfiguration();

        public int WritePageSize { get; set; } = 500;
        public int ReadPageSize { get; set; } = 500;
        public ISerializer Serializer { get; set; } = new JsonSerializer();
        public string ConnectionString { get; set; } = $"tcp://admin:changeit@localhost:1113";
        public ConnectionSettings ConnectionSettings { get; set; } = EventStoreConnectionSettings.Default;
        public UserCredentials UserCredentials { get; set; } = new UserCredentials("admin", "changeit");
        public int BufferSize { get; set; } = 10;
        public bool AutoAck { get; set; } = true;
    }
}
