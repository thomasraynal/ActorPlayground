using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IMessage
    {
        bool IsSystemMessage { get; }
    }
}
