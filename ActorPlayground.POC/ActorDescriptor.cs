using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class ActorDescriptor
    {
        public ActorDescriptor(string id, ActorType type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; }
        public ActorType Type { get; }
    }
}
