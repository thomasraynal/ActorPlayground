using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Stop : SystemMessage
    {
        public static Stop Instance => new Stop();
    }
}
