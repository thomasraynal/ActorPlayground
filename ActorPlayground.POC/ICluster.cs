using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ICluster: IActor
    {
        void Emit(string target, IMessage message);
        Task<T> Send<T>(string target, IMessage message);
        Task<T> Send<T>(string target, IMessage message, CancellationToken cancellationToken);
        Task<T> Send<T>(string target, IMessage message, TimeSpan timeout);
        IActorProcess Spawn(Func<IActor> actorFactory);
        void Start(string id);
        void Stop(string id);
        IActorProcess Get(string id);
        void Remove(string id);
    }
}