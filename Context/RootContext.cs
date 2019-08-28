﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{


    public class RootContext : IRootContext
    {
        public static readonly RootContext Empty = new RootContext();
        private Sender SenderMiddleware { get; set; }
        public MessageHeader Headers { get; private set; }

        public PID Parent { get => null; }
        public PID Self { get => null; }
        public PID Sender { get => null; }
        public IActor Actor { get => null; }

        public PID Spawn(Props props)
        {
            var name = ProcessRegistry.Instance.NextId();
            return SpawnNamed(props, name);
        }

        public PID SpawnNamed(Props props, string name)
        {
            var parent = props.GuardianStrategy != null ? Guardians.GetGuardianPID(props.GuardianStrategy) : null;
            return props.Spawn(name, parent);
        }

        public PID SpawnPrefix(Props props, string prefix)
        {
            var name = prefix + ProcessRegistry.Instance.NextId();
            return SpawnNamed(props, name);
        }

        public RootContext()
        {
            SenderMiddleware = null;
            Headers = MessageHeader.Empty;
        }

        public RootContext(MessageHeader messageHeader, params Func<Sender, Sender>[] middleware)
        {
            SenderMiddleware = middleware.Reverse()
                .Aggregate((Sender)DefaultSender, (inner, outer) => outer(inner));
            Headers = messageHeader;
        }

        public RootContext WithHeaders(MessageHeader headers) => Copy(c => c.Headers = headers);
        public RootContext WithSenderMiddleware(params Func<Sender, Sender>[] middleware) => Copy(c =>
        {
            SenderMiddleware = middleware.Reverse()
                .Aggregate((Sender)DefaultSender, (inner, outer) => outer(inner));
        });


        private RootContext Copy(Action<RootContext> mutator)
        {
            var copy = new RootContext
            {
                SenderMiddleware = SenderMiddleware,
                Headers = Headers
            };
            mutator(copy);
            return copy;
        }

        public object Message => null;


        private Task DefaultSender(ISenderContext context, PID target, MessageEnvelope message)
        {
            target.SendUserMessage(message);
            return Task.CompletedTask;
        }

        public void Send(PID target, object message)
            => SendUserMessage(target, message);

        public void Request(PID target, object message)
            => SendUserMessage(target, message);

        public void Request(PID target, object message, PID sender)
        {
            var envelope = new MessageEnvelope(message, sender, null);
            Send(target, envelope);
        }

        public Task<T> RequestAsync<T>(PID target, object message, TimeSpan timeout)
            => RequestAsync(target, message, new FutureProcess<T>(timeout));

        public Task<T> RequestAsync<T>(PID target, object message, CancellationToken cancellationToken)
            => RequestAsync(target, message, new FutureProcess<T>(cancellationToken));

        public Task<T> RequestAsync<T>(PID target, object message)
            => RequestAsync(target, message, new FutureProcess<T>());

        private Task<T> RequestAsync<T>(PID target, object message, FutureProcess<T> future)
        {
            var messageEnvelope = new MessageEnvelope(message, future.Pid, null);
            SendUserMessage(target, messageEnvelope);

            return future.Task;
        }

        private void SendUserMessage(PID target, object message)
        {
            if (SenderMiddleware != null)
            {
                //slow path
                SenderMiddleware(this, target, MessageEnvelope.Wrap(message));
            }
            else
            {
                //fast path, 0 alloc
                target.SendUserMessage(message);
            }
        }

        public void Stop(PID pid)
        {
            var reff = ProcessRegistry.Instance.Get(pid);
            reff.Stop(pid);
        }

        public Task StopAsync(PID pid)
        {
            var future = new FutureProcess<object>();

            Stop(pid);

            return future.Task;
        }

        public void Poison(PID pid) => pid.SendUserMessage(new PoisonPill());

        public Task PoisonAsync(PID pid)
        {
            var future = new FutureProcess<object>();

            Poison(pid);

            return future.Task;
        }
    }
}
