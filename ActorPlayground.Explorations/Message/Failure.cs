using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations.Message
{
    public class Failure : ISystemEvent
    {
        public Failure(string who, Exception reason)
        {
            Who = who;
            Reason = reason;
        }

        public string Who { get; }
        public Exception Reason { get; }

    }
}
