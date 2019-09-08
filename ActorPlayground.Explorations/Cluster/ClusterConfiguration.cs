using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public class ClusterConfiguration : RootRemoteConfiguration, IClusterConfiguration
    {
        public ClusterConfiguration(string name, string adress) : base(adress)
        {
            Name = name;
        }

        public ClusterConfiguration(string name, string adress, Func<IActor> actorFactory) : base(adress, actorFactory)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
