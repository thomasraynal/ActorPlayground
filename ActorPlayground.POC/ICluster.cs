using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ICluster: IActor
    {
        void Emit(string target, object message);
        Task<T> Send<T>(string target, object message);
        Task<T> Send<T>(string target, object message, CancellationToken cancellationToken);
        Task<T> Send<T>(string target, object message, TimeSpan timeout);
        ActorProcess Spawn(IActor actor);
        void Start(string id);
        void Stop(string id);
        ActorProcess Get(string id);
        void Remove(string id);
    }
}