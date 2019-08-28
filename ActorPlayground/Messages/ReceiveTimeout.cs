using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class ReceiveTimeout : SystemMessage
    {
        public static ReceiveTimeout Instance => new ReceiveTimeout();
    }
}
