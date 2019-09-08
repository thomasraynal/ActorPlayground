using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface IDirectory : IDisposable
    {
        Task Pulse(ActorId actorId);
        Task Register(IClusterMember member);
        Task Unregister(ActorId actorId);
        Task<IEnumerable<IClusterMember>> GetMembers();
    }
}
