using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Start: ISystemMessage
    {
        public Start(string who)
        {
            Who = who;
        }

        public string Who { get; }

        public bool IsSystemMessage => true;
    }
}
