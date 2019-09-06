using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IClusterProvider : IDisposable
    {
        Task Register(string clusterName, IClusterMember member);
        Task UnRegister(string clusterName, IClusterMember member);
    }
}
