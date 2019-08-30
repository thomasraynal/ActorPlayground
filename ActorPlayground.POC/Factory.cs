using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class Factory
    {
        public static IRoot Create(string adress)
        {
      
            var supervisorStrategy = new OneForOneStrategy();
            var serializer = new JsonNetSerializer();
            var registry = new LocalActorRegistry(supervisorStrategy, serializer);
            var cluster = new Root(registry, adress);

            return cluster;
        }

    }
}
