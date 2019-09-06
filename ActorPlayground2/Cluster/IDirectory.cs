using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IDirectory : IActorProcess, IDisposable
    {
        Task Pulse(string adress);
        Task Register(string clusterName, IClusterMember member);
        Task UnRegister(string clusterName, IClusterMember member);
    }
}
