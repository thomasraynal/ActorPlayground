using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class Factory
    {
        public static ICluster Create()
        {
      
            var supervisorStrategy = new OneForOneStrategy();
            var registry = new ActorRegistry(supervisorStrategy);
            var cluster = new Cluster(registry);

            return cluster;
        }

    }
}
