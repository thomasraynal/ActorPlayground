using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IDirectory : IDisposable
    {
        Task Pulse(string address);
        Task Register(IClusterMember member);
        Task Unregister(string address);
        Task<IEnumerable<IClusterMember>> GetMembers();
    }
}
