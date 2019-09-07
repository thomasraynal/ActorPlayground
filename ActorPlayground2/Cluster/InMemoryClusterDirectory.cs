using System;
using System.Collections.Generic;
using System.Linq;
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

    public class InMemoryClusterDirectory : IDirectory
    {
        private readonly Dictionary<string, ClusterMemberDescriptor> _world;
        private readonly IDirectoryConfiguration _configuration;

        public InMemoryClusterDirectory(IDirectoryConfiguration configuration)
        {
            _world = new Dictionary<string, ClusterMemberDescriptor>();
            _configuration = configuration;
        }

        public void Dispose()
        {
            _world.Clear();
        }

        public Task<IEnumerable<IClusterMember>> GetMembers()
        {
            return Task.FromResult(_world.Values.Select(v => v.ClusterMember));
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
            _world[member.ActorId.Adress] = new ClusterMemberDescriptor(member, _configuration.MemberTtl);

            return Task.CompletedTask;
        }

        public Task Unregister(string address)
        {
            _world.Remove(address);
            return Task.CompletedTask;
        }
    }
}
