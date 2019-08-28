using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public delegate SupervisorDirective Decider(PID pid, Exception reason);

}
