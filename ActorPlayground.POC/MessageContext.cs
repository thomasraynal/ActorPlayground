using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActorPlayground.POC
{
    internal readonly struct MessageContext : IContext
    {

        public MessageContext(ActorProcess actor, object message, ActorProcess sender)
        {
            Actor = actor;
            Message = message;
            Sender = sender;
        }

        public IEnumerable<string> Children => Actor.Children.Select(actor => actor.Id);

        public ActorProcess Actor { get; }

        public object Message { get; }

        public ActorProcess Sender { get; }

        public void Respond(object message)
        {
            Sender.Post(message, Actor);
        }
    }
}
