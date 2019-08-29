using System;

namespace ActorPlayground.POC
{
    public interface IActorRegistry
    {
        ActorProcess Add(Func<IActor> actorFactory, ActorProcess parent);
        ActorProcess Get(string id);
        void Remove(string id);
    }
}