using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICanSpawn
    {
        IActorProcess SpawnChild(Func<IActor> actorFactory);
    }
}
