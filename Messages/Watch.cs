using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Watch : SystemMessage
    {
        private PID self;

        public Watch(PID self)
        {
            this.self = self;
        }

        public PID Watcher { get; internal set; }
    }
}
