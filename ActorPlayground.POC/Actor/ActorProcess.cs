using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class ActorProcess : IActorProcess
    {

        private readonly IActorRegistry _registry;
        private readonly ISupervisorStrategy _supervisionStrategy;
        private readonly IMailbox _mailbox;
        private readonly Func<IActor> _actorFactory;
        private readonly IActorProcess _parent;

        public List<IActorProcess> Children { get; }
        public ActorId Id { get; private set; }
        public IActor Actor { get; private set; }

        public ActorType Type { get; private set; }

        public ActorProcess(
            ActorId id,
            Func<IActor> actorFactory,
            IActorProcess parent,
            IActorRegistry registry,
            ISupervisorStrategy supervisionStrategy)
        {
            _registry = registry;
            _supervisionStrategy = supervisionStrategy;
            _mailbox = new BlockingCollectionMailbox(this);
            _actorFactory = actorFactory;
            _parent = parent;

            Children = new List<IActorProcess>();
            Id = id;
        }

        public IActorProcess SpawnChild(Func<IActor> actorFactory)
        {
            var child = _registry.AddTransient(actorFactory, ActorType.Vanilla, _parent);

            Children.Add(child);

            return child;

        }
        public void Post(IMessage msg, IActorProcess sender)
        {
            _mailbox.Post(msg, sender);
        }

        public void Start()
        {
            Actor = _actorFactory();

            _mailbox.Start();
        }

        public void Stop()
        {
            _mailbox.Stop();
        }

        public async Task HandleFailure(Failure failure)
        {
            await _supervisionStrategy.HandleFailure(this, failure);
        }

        public void HandleSystemMessage(IMessage message)
        {
            switch (message)
            {
                case Start _:

                    Start();

                    foreach (var child in Children)
                    {
                        child.Start();
                    }

                    break;

                case Stop _:

                    Stop();

                    foreach (var child in Children)
                    {
                        child.Stop();
                    }

                    break;

                case Restart _:

                    Start();

                    foreach (var child in Children)
                    {
                        child.Start();
                    }

                    break;
            }
        }
    }
}
