using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActorPlayground.POC
{
    internal readonly struct MessageContext : IMessageContext
    {

        public MessageContext(IActorProcess actor, IEvent message, ICanPost sender, IActorRegistry actorRegistry)
        {
            Actor = actor;
            Message = message;
            Sender = sender;
            Registry = actorRegistry;
        }
        public IEnumerable<IActorProcess> Children => Actor.Children;

        public IActorProcess Actor { get; }

        public ICanPost Sender { get; }

        public IEvent Message { get; }

        public IActorRegistry Registry { get; }

        public void Respond(IEvent message)
        {
           Sender.Post(message, Actor);
        }
    }
}
