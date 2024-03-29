﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorProcess: ISupervisor
    {
        IReadOnlyList<IActorProcess> Children { get; }
        IActorProcessConfiguration Configuration { get; }
        IActor Actor { get; }
        void Post(IMessage msg, IActorProcess sender);
        IActorProcess SpawnChild(Func<IActor> actorFactory);
        void Start();
        void Stop();
        void HandleSystemMessage(IMessage message);
    }
}