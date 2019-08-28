using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    internal enum ContextState : byte
    {
        Alive,
        Restarting,
        Stopping,
        Stopped,
    }

}
