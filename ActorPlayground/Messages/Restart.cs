using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Restart : SystemMessage
    {
        private Exception reason;

        public Restart(Exception reason)
        {
            this.reason = reason;
        }
    }
}
