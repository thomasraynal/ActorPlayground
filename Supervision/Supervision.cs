using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public static class Supervision
    {
        public static ISupervisorStrategy DefaultStrategy { get; } =
            new OneForOneStrategy((who, reason) => SupervisorDirective.Restart);
    }

}
