using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IContext
    {
        IEnumerable<string> Children { get; }

        IMessage Message { get; }

        IActorProcess Sender { get; }

        void Respond(IMessage message);
    }
}
