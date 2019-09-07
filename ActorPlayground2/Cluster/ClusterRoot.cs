using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class ClusterRoot : Root, ICluster
    {

        private readonly IClusterRegistry _registry;

        public ClusterRoot(IClusterConfiguration configuration, IClusterRegistry registry): base(registry, configuration)
        {
            Configuration = configuration;
            _registry = registry;
        }

        public IClusterConfiguration Configuration { get; }

        public void Join(ActorId actorId)
        {
            _registry.Add(actorId);
        }

        public void Quit(ActorId actorId)
        {
            _registry.Remove(actorId);
        }
    }
}
