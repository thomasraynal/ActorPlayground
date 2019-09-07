using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class RootRemoteConfiguration : IRootRemoteConfiguration
    {
        public RootRemoteConfiguration(string adress)
        {
            Adress = adress;
            ActorFactory = () => EmptyActor.Instance;
        }

        public RootRemoteConfiguration(string adress, Func<IActor> actorFactory)
        {
            Adress = adress;
            ActorFactory = actorFactory;
        }

        public string Adress { get; }

        public Func<IActor> ActorFactory { get; }
    }
}
