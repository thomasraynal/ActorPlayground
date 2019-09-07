using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Start: ISystemEvent
    {
        public Start(string who)
        {
            Who = who;
        }

        public string Who { get; }

    }
}
