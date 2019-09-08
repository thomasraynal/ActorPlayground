using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface IRoot : IHasId, ICanSpawn, ICandSend, ICanEmit, IDisposable
    {

    }
}