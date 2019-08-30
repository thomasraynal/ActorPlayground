using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class ActorDescriptor
    {
        public ActorDescriptor(ActorId id, ActorType type)
        {
            Id = id;
            Type = type;
        }

        public ActorId Id { get; }
        public ActorType Type { get; }
    }
}
