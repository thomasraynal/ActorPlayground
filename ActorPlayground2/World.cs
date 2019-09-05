using StructureMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class World
    {

        public static IRoot Create<TRegistry>() where TRegistry : Registry
        {
            var container = new Container((conf) => conf.AddRegistry(Activator.CreateInstance<TRegistry>()));
            container.Configure(conf => conf.For<IRootRemoteConfiguration>().Use(new RootRemoteConfiguration(string.Empty)));
            return container.GetInstance<IRoot>();
        }

        public static IRoot Create<TRegistry>(IRootRemoteConfiguration rootRemoteConfiguration) where TRegistry : Registry
        {
            var container = new Container((conf) => conf.AddRegistry(Activator.CreateInstance<TRegistry>()));
            container.Configure(conf => conf.For<IRootRemoteConfiguration>().Use(rootRemoteConfiguration));
            return container.GetInstance<IRoot>();
        }

    }
}
