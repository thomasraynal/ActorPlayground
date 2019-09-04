using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IMessageContext
    {
        IEnumerable<IActorProcess> Children { get; }

        IEvent Message { get; }

        IActorRegistry Registry { get; }

        void Respond(IEvent message);
    }
}
