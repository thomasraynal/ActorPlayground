﻿using System;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorRegistry: IActor, IDisposable
    {
        IActorProcess Add(Func<IActor> actorFactory, ActorType type, IActorProcess parent);
        IActorProcess Add(Func<IActor> actorFactory, string adress, ActorType type, IActorProcess parent);
        IActorProcess Get(string id);
        void Remove(string id);
    }
}