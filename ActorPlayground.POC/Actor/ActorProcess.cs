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

        public List<IActorProcess> Children { get; }

        public IActor Actor { get; private set; }
        public IActorProcessConfiguration Configuration { get; private set; }

        public ActorProcess(
            IActorProcessConfiguration configuration,
            IActorRegistry registry,
            ISupervisorStrategy supervisionStrategy,
            ISerializer _)
        {
            _registry = registry;
            _supervisionStrategy = supervisionStrategy;
            _mailbox = new BlockingCollectionMailbox(this);

            Configuration = configuration;
            Children = new List<IActorProcess>();
     
        }

        public IActorProcess SpawnChild(Func<IActor> actorFactory)
        {
            var child = _registry.AddTransient(actorFactory, ActorType.Vanilla, Configuration.Parent);

            Children.Add(child);

            return child;

        }
        public void Post(IMessage msg, IActorProcess sender)
        {
            _mailbox.Post(msg, sender);
        }

        public void Start()
        {
            Actor = Configuration.ActorFactory();

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
