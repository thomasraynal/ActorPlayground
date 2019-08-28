using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public abstract class Process
    {
        protected internal abstract void SendUserMessage(PID pid, object message);

        public virtual void Stop(PID pid)
        {
            SendSystemMessage(pid, ActorPlayground.Stop.Instance);
        }

        protected internal abstract void SendSystemMessage(PID pid, object message);
    }

}
