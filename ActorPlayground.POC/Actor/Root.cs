using ActorPlayground.POC.Message;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Root : IRoot
    {
        private readonly IActorRegistry _registry;
        private readonly IActorProcess _process;
        
        public Root(IActorRegistry registry, string adress)
        {
            _registry = registry;
            _registry.Add(() => this, adress, ActorType.Root, null);
        }

        public IActorProcess Spawn(Func<IActor> actorFactory, string adress)
        {
            return  _registry.Add(actorFactory, adress, ActorType.Vanilla, _process);
        }

        public IActorProcess SpawnLocal(Func<IActor> actorFactory)
        {
            return _registry.AddTransient(actorFactory, ActorType.Vanilla, _process);
        }

        public void Emit(string target, IMessage message)
        {
            var process = _registry.Get(target);
            process.Post(message, _process);
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
            var futureProcess = _registry.AddTransient(() => future, ActorType.Future, _process);

            targetProcess.Post(message, futureProcess);

            return future.UnderlyingTask;
        }

        public IActorProcess Get(string id)
        {
            return _registry.Get(id);
        }

        public void Stop(string id)
        {
            Emit(id, new Stop(id));
        }

        public void Start(string id)
        {
            Emit(id, new Start(id));
        }

        public void Remove(string id)
        {
            Stop(id);

            _registry.Remove(id);
        }

        public Task Receive(IContext context)
        {
            return Task.CompletedTask;
        }

    }
}
