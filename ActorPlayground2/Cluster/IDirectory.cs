using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IDirectory : IDisposable
    {
        Task Pulse(ActorId actorId);
        Task Register(IClusterMember member);
        Task Unregister(ActorId actorId);
        Task<IEnumerable<IClusterMember>> GetMembers();
    }
}
