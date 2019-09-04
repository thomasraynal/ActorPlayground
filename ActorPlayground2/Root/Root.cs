using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Root : IRoot
    {
        private readonly IActorRegistry _registry;
        private readonly IActorProcess _process;

        public Root(IActorRegistry registry, IRootRemoteConfiguration rootConfiguration)
        {
            _registry = registry;

            if (string.IsNullOrEmpty(rootConfiguration.Adress))
            {
                _process = _registry.Add(() => this, ActorType.Transient, null);
            }
            else
            {
                _process = _registry.Add(() => this, rootConfiguration.Adress, ActorType.Remote, null);
            }
        }

        public IActorProcess Spawn(Func<IActor> actorFactory, string adress)
        {
            return _registry.Add(actorFactory, adress, ActorType.Remote, null);
        }

        public IActorProcess Spawn(Func<IActor> actorFactory)
        {
            return _registry.Add(actorFactory, ActorType.Transient, null);
        }

        public void Emit(string targetId, IEvent message)
        {
            var process = _registry.Get(targetId);
            process.Post(message, null);
        }

        public Task<T> Send<T>(string targetId, IEvent message, TimeSpan timeout) where T : IEvent
        {
            return SendInternal(targetId, message, new Future<T>(timeout));
        }

        public Task<T> Send<T>(string targetId, IEvent message, CancellationToken cancellationToken) where T : IEvent
        {
            return SendInternal(targetId, message, new Future<T>(cancellationToken));
        }

        public Task<T> Send<T>(string targetId, IEvent message) where T : IEvent
        {
            return SendInternal(targetId, message, new Future<T>());
        }

        private Task<T> SendInternal<T>(string targetId, IEvent message, Future<T> future) where T : IEvent
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

        public Task Receive(IMessageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
