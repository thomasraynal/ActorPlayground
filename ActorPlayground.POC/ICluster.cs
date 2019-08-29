using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ICluster
    {
        void Emit(string target, object message);
        Task<T> Send<T>(string target, object message);
        Task<T> Send<T>(string target, object message, CancellationToken cancellationToken);
        Task<T> Send<T>(string target, object message, TimeSpan timeout);
        ActorProcess Spawn(Func<IActor> actorFactory);
        void Start(string id);
        void Stop(string id);
        ActorProcess Get(string id);
        void Remove(string id);
    }
}