using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Restart : ISystemEvent
    {

        public Restart(string who, Exception reason)
        {
            Reason = reason;
            Who = who;
        }

        public Exception Reason { get; }
        public string Who { get; }

        public bool IsSystemMessage => true;
    }
}
