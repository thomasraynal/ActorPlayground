using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Message
{
    public class Remove : ISystemMessage
    {
        public Remove(string who)
        {
            Who = who;
        }

        public string Who { get; }
    }
}
