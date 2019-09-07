using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class ClusterRoot : Root, ICluster
    {
        private IClusterConfiguration _configuration;
        private IActorRegistry _registry;

        public ClusterRoot(IClusterConfiguration configuration, IActorRegistry registry): base(registry, configuration)
        {
            _configuration = configuration;
            _registry = registry;
        }

    }
}
