using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IRoot: IActor
    {
        void Emit(string target, IMessage message);
        Task<T> Send<T>(string target, IMessage message);
        Task<T> Send<T>(string target, IMessage message, CancellationToken cancellationToken);
        Task<T> Send<T>(string target, IMessage message, TimeSpan timeout);
        IActorProcess Spawn(Func<IActor> actorFactory, string adress);
        IActorProcess SpawnTransient(Func<IActor> actorFactory);
    }
}