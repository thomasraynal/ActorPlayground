using ActorPlayground.POC.Message;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class World : IWorld
    {
        private readonly IActorRegistry _registry;
        private readonly IActorProcess _process;

        public World(IActorRegistry registry)
        {
            _registry = registry;

        }

        public IActorProcess Spawn(Func<IActor> actorFactory, string adress)
        {
            return _registry.Add(actorFactory, adress, ActorType.Remote, _process);
        }

        public IActorProcess Spawn(Func<IActor> actorFactory)
        {
            return _registry.Add(actorFactory, ActorType.Transient, _process);
        }

        public void Emit(string target, IMessage message)
        {
            var process = _registry.Get(target);
            process.Post(message, _process);
        }

        public void Emit(IMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<T> Send<T>(string target, IMessage message, TimeSpan timeout)
        {
            return SendInternal(target, message, new Future<T>(timeout));
        }

        public Task<T> Send<T>(string target, IMessage message, CancellationToken cancellationToken)
        {
            return SendInternal(target, message, new Future<T>(cancellationToken));
        }

        public Task<T> Send<T>(string target, IMessage message)
        {
            return SendInternal(target, message, new Future<T>());
        }

        private Task<T> SendInternal<T>(string target, IMessage message, Future<T> future)
        {
            var targetProcess = _registry.Get(target);
            var futureProcess = _registry.Add(() => future, ActorType.Future, _process);

            targetProcess.Post(message, futureProcess);

            return future.UnderlyingTask;
        }

        public void Dispose()
        {
            _registry.Dispose();
        }

       
    }
}
