using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Pulse : IEvent
    {
        public Pulse(ActorId actorId)
        {
            ActorId = actorId;
        }

        ActorId ActorId { get; }
    }
}
