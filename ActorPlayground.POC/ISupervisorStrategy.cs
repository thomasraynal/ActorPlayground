using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ISupervisorStrategy
    {
        Task Handle(ISystemMessage message);
    }
}
