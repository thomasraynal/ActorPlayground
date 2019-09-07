using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IRoot : IHasId, ICanSpawn, ICandSend, ICanEmit, IDisposable
    {

    }
}