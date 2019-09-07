using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Unregister : IEvent
    {
        public Unregister(ActorId actorId)
        {
            ActorId = actorId;
        }

        public ActorId ActorId { get; }
    }
}
