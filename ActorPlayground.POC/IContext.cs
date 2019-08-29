using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IContext
    {
        IEnumerable<string> Children { get; }

        object Message { get; }

        ActorProcess Sender { get; }

        void Respond(object message);
    }
}
