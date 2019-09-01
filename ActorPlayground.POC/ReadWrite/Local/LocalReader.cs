using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class LocalReader : IReader
    {
        private readonly IActorProcess _sender;
        private readonly IActorProcess _target;

        public LocalReader(ActorId actorId, IActorRegistry registry, IActorProcess sender)
        {
            _target = registry.Get(actorId.Value);
            _sender = sender;
        }

        public void Post(IMessage msg)
        {
            _target.Post(msg, _sender);
        }
    }
}
