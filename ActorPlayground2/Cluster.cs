using StructureMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class Cluster
    {

        public static ICluster Create<TRegistry>(IClusterConfiguration rootRemoteConfiguration, IDirectoryConfiguration directoryConfiguration) where TRegistry : Registry
        {
            var container = new Container((conf) => conf.AddRegistry(Activator.CreateInstance<TRegistry>()));
            container.Configure(conf => conf.For<IClusterConfiguration>().Use(rootRemoteConfiguration));
            container.Configure(conf => conf.For<IDirectoryConfiguration>().Use(directoryConfiguration));
            return container.GetInstance<ICluster>();
        }

        public static void Join(string clusterDirectoryAdress, ActorId actorId)
        {

        }

    }
}
