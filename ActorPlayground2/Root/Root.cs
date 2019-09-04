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

        public void Emit(string targetId, IMessage message)
        {
            var process = _registry.Get(targetId);
            process.Post(message, null);
        }

        public Task<T> Send<T>(string targetId, IMessage message, TimeSpan timeout) where T: IMessage
        {
            return SendInternal(targetId, message, new Future<T>(timeout));
        }

        public Task<T> Send<T>(string targetId, IMessage message, CancellationToken cancellationToken) where T : IMessage
        {
            return SendInternal(targetId, message, new Future<T>(cancellationToken));
        }

        public Task<T> Send<T>(string targetId, IMessage message) where T : IMessage
        {
            return SendInternal(targetId, message, new Future<T>());
        }

        private Task<T> SendInternal<T>(string targetId, IMessage message, Future<T> future) where T : IMessage
        {
            var targetProcess = _registry.Get(targetId);
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
