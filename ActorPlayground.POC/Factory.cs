using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class Factory
    {
        public static ICluster Create()
        {
            var cluster = new Cluster();
            var supervisor = new Supervisor(new OneForOneStrategy(cluster));
            var registry = new ActorRegistry(supervisor);

            supervisor.Initialize(registry);
            registry.Initialize(cluster);

            cluster.Initialize(supervisor, registry);

            return cluster;
        }

    }
}
