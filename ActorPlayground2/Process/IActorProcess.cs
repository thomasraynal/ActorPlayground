using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorProcess : ICanPost, ICandSend, ICanEmit, ISupervisor, IProcess
    {
        IReadOnlyList<IActorProcess> Children { get; }
        IActorProcessConfiguration Configuration { get; }
        IActor Actor { get; }
        IActorProcess SpawnChild(Func<IActor> actorFactory);
        void HandleSystemMessage(IEvent message);
        void HandleCommandResult(ICommandResult message);

    }
}