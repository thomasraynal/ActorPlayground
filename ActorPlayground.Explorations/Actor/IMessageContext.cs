using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public interface IMessageContext
    {
        IEnumerable<IActorProcess> Children { get; }

        IEvent Message { get; }

        ICanPost Sender { get; }

        IActorRegistry Registry { get; }

        void Respond(IEvent message);
    }
}
