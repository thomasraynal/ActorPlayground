using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class SuspendMailbox : SystemMessage
    {
        public static SuspendMailbox Instance => new SuspendMailbox();
    }
}
