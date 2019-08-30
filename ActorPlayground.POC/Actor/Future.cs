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

        public Future()
        {
            _task = new TaskCompletionSource<T>();

            UnderlyingTask = _task.Task;
        }

        public Future(TimeSpan timeout)
        {
            _task = new TaskCompletionSource<T>();

            var cancel = new CancellationTokenSource(timeout);

            cancel.Token.Register(() => _task.TrySetCanceled(), false);

            UnderlyingTask = _task.Task.ContinueWith(result => result.Result, cancel.Token);
        }

        public Future(CancellationToken cancellationToken)
        {
            _task = new TaskCompletionSource<T>();

            UnderlyingTask = _task.Task.ContinueWith(result => result.Result, cancellationToken);
        }

        public Task<T> UnderlyingTask { get; }

        public Task Receive(IContext context)
        {
            _task.TrySetResult((T)context.Message);

            return Task.CompletedTask;
        }
    }
}
