using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Future<T> : IActor
    {
        private readonly TaskCompletionSource<T> _task;
        private readonly IActorProcess _underlying;

        public Future(IActorProcess underlying)
        {
            _task = new TaskCompletionSource<T>();
            _underlying = underlying;

            UnderlyingTask = _task.Task;
        }

        public Future(IActorProcess underlying, TimeSpan timeout)
        {
            _task = new TaskCompletionSource<T>();
            _underlying = underlying;

            var cancel = new CancellationTokenSource(timeout);

            cancel.Token.Register(() => _task.TrySetCanceled(), false);

            UnderlyingTask = _task.Task.ContinueWith(result => result.Result, cancel.Token);
        }

        public Future(IActorProcess underlying, CancellationToken cancellationToken)
        {
            _task = new TaskCompletionSource<T>();
            _underlying = underlying;

            UnderlyingTask = _task.Task.ContinueWith(result => result.Result, cancellationToken);
        }

        public Task<T> UnderlyingTask { get; }

        public Task Receive(IMessageContext context)
        {
            _task.TrySetResult((T)context.Message);

            _underlying.Post(context.Message, context.Sender);

            return Task.CompletedTask;
        }
    }
}
