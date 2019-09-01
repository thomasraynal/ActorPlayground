using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class LocalWriter : IWriter
    {
        private readonly IActorRegistry _registry;
        private readonly IActorProcess _process;

        public LocalWriter(IActorRegistry registry, IActorProcess process)
        {
            _registry = registry;
            _process = process;
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
    }
}
