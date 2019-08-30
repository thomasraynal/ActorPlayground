using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ISystemMessage : IMessage
    {
        string Who { get; }
    }
}
