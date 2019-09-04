using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICanPost
    {
        ActorId Id { get; }
        void Post(IMessage msg, ICanPost sender);
    }
}
