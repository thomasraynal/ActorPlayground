using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICanSpawn
    {
        IActorProcess Spawn(Func<IActor> actorFactory, string adress);
        IActorProcess Spawn(Func<IActor> actorFactory);

        IActorProcess SpawnNamed(Func<IActor> actorFactory, string adress, string name);
        IActorProcess SpawnNamed(Func<IActor> actorFactory, string name);
    }
}
