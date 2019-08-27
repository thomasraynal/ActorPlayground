using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public interface IContext : ISenderContext, IReceiverContext, ISpawnerContext, IStopperContext
    {
        TimeSpan ReceiveTimeout { get; }
        IReadOnlyCollection<PID> Children { get; }
        void Respond(object message);
        void Stash();
        void Watch(PID pid);
        void Unwatch(PID pid);
        void SetReceiveTimeout(TimeSpan duration);
        void CancelReceiveTimeout();
        void Forward(PID target);
        void ReenterAfter<T>(Task<T> target, Func<Task<T>, Task> action);
        void ReenterAfter(Task target, Action action);
    }
}
