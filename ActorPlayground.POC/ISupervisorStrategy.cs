using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ISupervisorStrategy
    {
        Task Handle(ISystemMessage message);
    }
}
