﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IRoot : ICandSend, ICanEmit, IDisposable
    {
        IActorProcess Spawn(Func<IActor> actorFactory, string adress);
        IActorProcess Spawn(Func<IActor> actorFactory);
    }
}