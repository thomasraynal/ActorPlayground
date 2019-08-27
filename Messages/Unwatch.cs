using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Unwatch : SystemMessage
    {
        private PID self;

        public Unwatch(PID self)
        {
            this.self = self;
        }

        public PID Watcher { get; internal set; }
    }
}
