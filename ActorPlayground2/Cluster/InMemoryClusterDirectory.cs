using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    internal class ClusterMemberDescriptor
    {
        public ClusterMemberDescriptor(IClusterMember clusterMember, TimeSpan ttl)
        {
            ClusterMember = clusterMember;
            Ttl = ttl;
        }

        public IClusterMember ClusterMember { get; }
        public TimeSpan Ttl { get; }
        public DateTime LastPulse { get; set; }
        public bool IsAlive => LastPulse.Add(Ttl) < DateTime.Now;

    }

    public class InMemoryClusterDirectory : ActorProcess, IDirectory
    {
        private readonly Dictionary<string, ClusterMemberDescriptor> _world;

        public InMemoryClusterDirectory(IActorProcessConfiguration configuration, IActorRegistry registry, ISupervisorStrategy supervisionStrategy) : base(configuration, registry, supervisionStrategy)
        {
            _world = new Dictionary<string, ClusterMemberDescriptor>();
        }

        public void Dispose()
        {
            _world.Clear();
        }

        public Task Pulse(string adress)
        {
            if (_world.ContainsKey(adress))
            {
                _world[adress].LastPulse = DateTime.Now;
            }

            return Task.CompletedTask;

        }

        public Task Register(IClusterMember member)
        {
            throw new NotImplementedException();
        }

        public Task UnRegister(IClusterMember member)
        {
            throw new NotImplementedException();
        }
    }
}
