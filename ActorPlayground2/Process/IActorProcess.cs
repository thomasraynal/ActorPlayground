using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorProcess: ICanPost, ICanSpawn, ISupervisor
    {
        IReadOnlyList<IActorProcess> Children { get; }
        IActorProcessConfiguration Configuration { get; }
        IActor Actor { get; }
        void Start();
        void Stop();
        void HandleSystemMessage(IMessage message);
    }
}