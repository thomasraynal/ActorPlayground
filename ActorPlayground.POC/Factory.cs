using StructureMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class Factory
    {
        public static IRoot Create<TRegistry>(IRootConfiguration configuration) where TRegistry : Registry
        {
            var container = new Container((conf) => conf.AddRegistry(Activator.CreateInstance<TRegistry>()));
            container.Configure(conf => conf.For<IRootConfiguration>().Use(configuration));
            return container.GetInstance<IRoot>();
        }

    }
}
