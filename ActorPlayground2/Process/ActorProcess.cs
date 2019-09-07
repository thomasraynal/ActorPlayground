using ActorPlayground.POC.Message;
using ActorPlayground.POC.Remote;
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
        private readonly List<IActorProcess> _children;
        private readonly CommandHandler _commandHandler;
        public IReadOnlyList<IActorProcess> Children => _children;
        public IActor Actor { get; private set; }
        public IActorProcessConfiguration Configuration { get; private set; }
        public ActorId Id => Configuration.Id;

        public ActorProcess(
            IActorProcessConfiguration configuration,
            IActorRegistry registry,
            ISupervisorStrategy supervisionStrategy)
        {
            _registry = registry;
            _supervisionStrategy = supervisionStrategy;
            _mailbox = new BlockingCollectionMailbox(this, registry);
            _children = new List<IActorProcess>();
            _commandHandler = new CommandHandler(this, _registry);

            Configuration = configuration;

        }

        public IActorProcess SpawnChild(Func<IActor> actorFactory)
        {
            var child = _registry.Add(actorFactory, Configuration.Parent);

            _children.Add(child);

            return child;

        }
        public void Post(IEvent msg, ICanPost sender)
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

        public void HandleSystemMessage(IEvent message)
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

        public void Emit(string targetId, IEvent message)
        {
            var process = _registry.Get(targetId);
            process.Post(message, null);
        }

        //refacto: default timeout
        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message, TimeSpan timeout) where TCommandResult : ICommandResult
        {
            return _commandHandler.Send<TCommandResult>(targetId, message, timeout);
        }

        //refacto: default timeout
        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message, CancellationToken cancellationToken) where TCommandResult : ICommandResult
        {
            return _commandHandler.Send<TCommandResult>(targetId, message, cancellationToken);
        }

        //refacto: default timeout
        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message) where TCommandResult : ICommandResult
        {
            return _commandHandler.Send<TCommandResult>(targetId, message);
        }

        public void HandleCommandResult(ICommandResult message)
        {
            _commandHandler.HandleCommandResult(message);
        }
    }
}
