using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Register : IEvent
    {
        public Register(ActorId actorId)
        {
            ActorId = actorId;
        }

        public ActorId ActorId { get; }
    }
}
