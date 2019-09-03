using StructureMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public static class Factory
    {
        public static IRoot Create<TRegistry>() where TRegistry : Registry
        {
            var container = new Container((conf) => conf.AddRegistry(Activator.CreateInstance<TRegistry>()));
            return container.GetInstance<IRoot>();
        }

    }
}
