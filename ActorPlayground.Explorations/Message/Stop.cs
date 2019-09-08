using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations.Message
{
    public class Stop : ISystemEvent
    {
        public Stop(string who)
        {
            Who = who;
        }

        public string Who { get; }

    }
}
