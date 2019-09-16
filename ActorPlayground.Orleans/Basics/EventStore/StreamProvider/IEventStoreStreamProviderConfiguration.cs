using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IEventStoreStreamProviderConfiguration : IEventStoreRepositoryConfiguration
    {
        IList<string> Groups { get; }
    }
}
