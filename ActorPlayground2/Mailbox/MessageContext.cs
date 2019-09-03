using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActorPlayground.POC
{
    internal readonly struct MessageContext : IContext
    {

        public MessageContext(IActorProcess actor, IMessage message, ICanPost sender, IActorRegistry actorRegistry)
        {
            Actor = actor;
            Message = message;
            Sender = sender;
            Registry = actorRegistry;
        }
        public IEnumerable<IActorProcess> Children => Actor.Children;

        public IActorProcess Actor { get; }

        public ICanPost Sender { get; }

        public IMessage Message { get; }

        public IActorRegistry Registry { get; }

        public void Respond(IMessage message)
        {
           Sender.Post(message, Actor);
        }
    }
}
