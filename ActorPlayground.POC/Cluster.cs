using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Cluster : IActor
    {
        private IActorRegistry _registry;
        private ActorProcess _process;

        public ActorProcess Spawn(IActor actor)
        {
            return _registry.Add(actor);
        }

        public Cluster(IActorRegistry registry)
        {
            _registry = registry;
            _process = _registry.Add(this);
        }

        public void Emit(string target, object message)
        {
            var process = _registry.Get(target);
            process.Post(message, _process);
        }

        public Task<T> Send<T>(string target, object message, TimeSpan timeout)
        {
            return SendInternal(target, message, new Future<T>(timeout));
        }

        public Task<T> Send<T>(string target, object message, CancellationToken cancellationToken)
        {
            return SendInternal(target, message, new Future<T>(cancellationToken));
        }

        public Task<T> Send<T>(string target, object message)
        {
            return SendInternal(target, message, new Future<T>());
        }

        private Task<T> SendInternal<T>(string target, object message, Future<T> future)
        {
            var targetProcess = _registry.Get(target);
            var futureProcess = _registry.Add(future);

            targetProcess.Post(message, futureProcess);

            return future.UnderlyingTask;
        }

        public Task Stop(string pid)
        {
            return Task.CompletedTask;
        }

        public Task Receive(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
