using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Failure : SystemMessage
    {
        private PID self;

        public Failure(PID self, Exception reason, object message)
        {
            this.self = self;
            Reason = reason;
            Message = message;
        }

        public PID Who { get; internal set; }
        public Exception Reason { get; internal set; }
        public object Message { get; internal set; }
    }
}
