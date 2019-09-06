using StructureMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class Cluster
    {
        public static ICluster Create<TRegistry>() where TRegistry : Registry
        {
            throw new NotImplementedException();
        }

    }
}
