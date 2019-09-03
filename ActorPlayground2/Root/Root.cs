using ActorPlayground.POC.Message;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Root : IRoot
    {
        private readonly IActorRegistry _registry;

        public Root(IActorRegistry registry)
        {
            _registry = registry;
        }

        public IActorProcess Spawn(Func<IActor> actorFactory, string adress)
        {
            return _registry.Add(actorFactory, adress, ActorType.Remote, null);
        }

        public IActorProcess Spawn(Func<IActor> actorFactory)
        {
            return _registry.Add(actorFactory, ActorType.Transient, null);
        }

        public void Emit(string target, IMessage message)
        {
            var process = _registry.Get(target);
            process.Post(message, null);
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
            var futureProcess = _registry.Add(() => future, ActorType.Future, null);

            targetProcess.Post(message, futureProcess);

            return future.UnderlyingTask.ContinueWith(task =>
            {
                _registry.Remove(futureProcess.Configuration.Id.Value);
                return task.Result;
            });
        }

        public void Dispose()
        {
            _registry.Dispose();
        }

       
    }
}
