using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ICluster : ICanSpawn, ICandSend, ICanEmit, IDisposable
    {
        IClusterConfiguration Configuration { get; }
        void Join(ActorId actorId);
        void Quit(ActorId actorId);
    }
}
