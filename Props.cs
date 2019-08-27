﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    static class Middleware
    {
        internal static Task Receive(IReceiverContext context, MessageEnvelope envelope)
        {
            return context.Receive(envelope);
        }

        internal static Task Sender(ISenderContext context, PID target, MessageEnvelope envelope)
        {
            target.SendUserMessage(envelope);
            return Task.CompletedTask;
        }
    }
    public sealed class Props
    {
        private Spawner _spawner;
        public Func<IActor> Producer { get; private set; }
        public Func<IMailbox> MailboxProducer { get; private set; } = ProduceDefaultMailbox;
        public ISupervisorStrategy GuardianStrategy { get; private set; }
        public ISupervisorStrategy SupervisorStrategy { get; private set; } = Supervision.DefaultStrategy;
        public IDispatcher Dispatcher { get; private set; } = Dispatchers.DefaultDispatcher;
        public IList<Func<Receiver, Receiver>> ReceiverMiddleware { get; private set; } = new List<Func<Receiver, Receiver>>();
        public IList<Func<Sender, Sender>> SenderMiddleware { get; private set; } = new List<Func<Sender, Sender>>();
        public Receiver ReceiverMiddlewareChain { get; private set; }
        public Sender SenderMiddlewareChain { get; private set; }
        public IList<Func<IContext, IContext>> ContextDecorator { get; private set; } = new List<Func<IContext, IContext>>();
        public Func<IContext, IContext> ContextDecoratorChain { get; private set; } = DefaultContextDecorator;

        public Spawner Spawner
        {
            get => _spawner ?? DefaultSpawner;
            private set => _spawner = value;
        }

        private static IContext DefaultContextDecorator(IContext context) => context;

        private static IMailbox ProduceDefaultMailbox() => UnboundedMailbox.Create();

        private static PID DefaultSpawner(string name, Props props, PID parent)
        {
            var ctx = new ActorContext(props, parent);
            var mailbox = props.MailboxProducer();
            var dispatcher = props.Dispatcher;
            var process = new ActorProcess(mailbox);
            var (pid, absent) = ProcessRegistry.Instance.TryAdd(name, process);
            if (!absent)
            {
                throw new Exception($"{name} {pid}");
            }
            ctx.Self = pid;
            mailbox.RegisterHandlers(ctx, dispatcher);
            mailbox.PostSystemMessage(Started.Instance);
            mailbox.Start();

            return pid;
        }

        public Props WithProducer(Func<IActor> producer) => Copy(props => props.Producer = producer);

        public Props WithDispatcher(IDispatcher dispatcher) => Copy(props => props.Dispatcher = dispatcher);

        public Props WithMailbox(Func<IMailbox> mailboxProducer) => Copy(props => props.MailboxProducer = mailboxProducer);

        public Props WithContextDecorator(params Func<IContext, IContext>[] contextDecorator) => Copy(props =>
        {
            props.ContextDecorator = ContextDecorator.Concat(contextDecorator).ToList();
            props.ContextDecoratorChain = props.ContextDecorator.Reverse()
                                               .Aggregate((Func<IContext, IContext>)DefaultContextDecorator, (inner, outer) => ctx => outer(inner(ctx)));
        });

        public Props WithGuardianSupervisorStrategy(ISupervisorStrategy guardianStrategy) => Copy(props => props.GuardianStrategy = guardianStrategy);

        public Props WithChildSupervisorStrategy(ISupervisorStrategy supervisorStrategy) => Copy(props => props.SupervisorStrategy = supervisorStrategy);

        public Props WithReceiverMiddleware(params Func<Receiver, Receiver>[] middleware) => Copy(props =>
        {
            props.ReceiverMiddleware = ReceiverMiddleware.Concat(middleware).ToList();
            props.ReceiverMiddlewareChain = props.ReceiverMiddleware.Reverse()
                                                .Aggregate((Receiver)Middleware.Receive, (inner, outer) => outer(inner));
        });

        public Props WithSenderMiddleware(params Func<Sender, Sender>[] middleware) => Copy(props =>
        {
            props.SenderMiddleware = SenderMiddleware.Concat(middleware).ToList();
            props.SenderMiddlewareChain = props.SenderMiddleware.Reverse()
                                               .Aggregate((Sender)Middleware.Sender, (inner, outer) => outer(inner));
        });

        public Props WithSpawner(Spawner spawner) => Copy(props => props.Spawner = spawner);

        private Props Copy(Action<Props> mutator)
        {
            var props = new Props
            {
                Dispatcher = Dispatcher,
                MailboxProducer = MailboxProducer,
                Producer = Producer,
                ReceiverMiddleware = ReceiverMiddleware,
                ReceiverMiddlewareChain = ReceiverMiddlewareChain,
                SenderMiddleware = SenderMiddleware,
                SenderMiddlewareChain = SenderMiddlewareChain,
                Spawner = Spawner,
                SupervisorStrategy = SupervisorStrategy,
                GuardianStrategy = GuardianStrategy,
                ContextDecorator = ContextDecorator,
                ContextDecoratorChain = ContextDecoratorChain,
            };
            mutator(props);
            return props;
        }

        internal PID Spawn(string name, PID parent) => Spawner(name, this, parent);
        public static Props FromProducer(Func<IActor> producer) => new Props().WithProducer(producer);
    }

    public delegate PID Spawner(string id, Props props, PID parent);
}
