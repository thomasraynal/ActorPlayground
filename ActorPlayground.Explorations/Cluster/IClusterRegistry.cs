using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface IClusterRegistry : IActorRegistry
    {
        IActorProcess Add(ActorId actorId);
    }
}
