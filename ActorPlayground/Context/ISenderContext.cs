using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public interface ISenderContext : IInfoContext
    {
        void Send(PID target, object message);
        void Request(PID target, object message);
        void Request(PID target, object message, PID sender);
        Task<T> RequestAsync<T>(PID target, object message, TimeSpan timeout);
        Task<T> RequestAsync<T>(PID target, object message, CancellationToken cancellationToken);
        Task<T> RequestAsync<T>(PID target, object message);
        MessageHeader Headers { get; }

        //TODO: should the current message of the actor be exposed to sender middleware?
        object Message { get; }

    }
}
