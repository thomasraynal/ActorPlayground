using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class Factory
    {
        public static ICluster Create(string adress)
        {
      
            var supervisorStrategy = new OneForOneStrategy();
            var registry = new LocalActorRegistry(supervisorStrategy);
            var cluster = new Cluster(registry, adress);

            return cluster;
        }

    }
}
