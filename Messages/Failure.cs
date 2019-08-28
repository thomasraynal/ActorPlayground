using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Failure : SystemMessage
    {
        public Failure(PID who, Exception reason, object message)
        {
            Who = who;
            Reason = reason;
            Message = message;
        }

        public PID Who { get; internal set; }
        public Exception Reason { get; internal set; }
        public object Message { get; internal set; }
    }
}
