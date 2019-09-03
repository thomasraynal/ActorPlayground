using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IRoot : IDisposable
    {
        void Emit(IMessage message);
        void Emit(string targetId, IMessage message);
        Task<T> Send<T>(string targetId, IMessage message);
        Task<T> Send<T>(string targetId, IMessage message, CancellationToken cancellationToken);
        Task<T> Send<T>(string targetId, IMessage message, TimeSpan timeout);
        IActorProcess Spawn(Func<IActor> actorFactory, string adress);
        IActorProcess Spawn(Func<IActor> actorFactory);
    }
}