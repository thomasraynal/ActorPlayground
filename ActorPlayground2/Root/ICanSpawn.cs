using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICanSpawn
    {
        IActorProcess Spawn(Func<IActor> actorFactory, string adress);
        IActorProcess Spawn(Func<IActor> actorFactory);
    }
}
