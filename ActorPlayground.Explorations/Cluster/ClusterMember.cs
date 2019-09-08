using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public class ClusterMember : IClusterMember
    {

        public ClusterMember(ActorId actorId)
        {
            ActorId = actorId;
        }

        public ActorId ActorId { get; }
    }
}
