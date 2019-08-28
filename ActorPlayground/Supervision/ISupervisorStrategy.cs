using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public interface ISupervisorStrategy
    {
        void HandleFailure(ISupervisor supervisor, PID child, Exception cause, object message);
    }
}
