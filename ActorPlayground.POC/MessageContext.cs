using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActorPlayground.POC
{
    internal readonly struct MessageContext : IContext
    {

        public MessageContext(IActorProcess actor, IMessage message, IActorProcess sender)
        {
            Actor = actor;
            Message = message;
            Sender = sender;
        }
        public IEnumerable<string> Children => Actor.Children.Select(actor => actor.Id.Value);

        public IActorProcess Actor { get; }

        public IMessage Message { get; }

        public IActorProcess Sender { get; }

        public void Respond(IMessage message)
        {
            Sender.Post(message, Actor);
        }
    }
}
