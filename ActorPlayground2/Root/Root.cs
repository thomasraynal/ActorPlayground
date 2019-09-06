using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Root : IRoot
    {
        private readonly IActorRegistry _registry;
        private readonly IActorProcess _process;

        public Root(IActorRegistry registry, IRootRemoteConfiguration rootConfiguration)
        {
            _registry = registry;

            if (string.IsNullOrEmpty(rootConfiguration.Adress))
            {
                _process = _registry.Add(rootConfiguration.ActorFactory, ActorType.Transient, null);
            }
            else
            {
                _process = _registry.Add(rootConfiguration.ActorFactory, rootConfiguration.Adress, ActorType.Remote, null);
            }

        }

        public IActorProcess Spawn(Func<IActor> actorFactory, string adress)
        {
            return _registry.Add(actorFactory, adress, ActorType.Remote, null);
        }

        public IActorProcess Spawn(Func<IActor> actorFactory)
        {
            return _registry.Add(actorFactory, ActorType.Transient, null);
        }

        public void Emit(string targetId, IEvent message)
        {
            var process = _registry.Get(targetId);
            process.Post(message, null);
        }

        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message, TimeSpan timeout) where TCommandResult : ICommandResult
        {
            return _process.Send<TCommandResult>(targetId, message, timeout);
        }

        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message, CancellationToken cancellationToken) where TCommandResult : ICommandResult
        {
            return _process.Send<TCommandResult>(targetId, message, cancellationToken);
        }

        //refacto: default timeout
        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message) where TCommandResult : ICommandResult
        {
            return _process.Send<TCommandResult>(targetId, message);
        }

        public void Dispose()
        {
            _registry.Dispose();
        }

    }
}
