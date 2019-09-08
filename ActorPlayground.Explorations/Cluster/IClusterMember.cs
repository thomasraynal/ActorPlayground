using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public interface IClusterMember
    {
        ActorId ActorId { get; }
    }
}
