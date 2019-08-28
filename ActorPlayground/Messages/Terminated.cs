using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Terminated : SystemMessage
    {
        public PID Who { get; internal set; }

        internal static object From(PID self)
        {
            throw new NotImplementedException();
        }
    }
}
