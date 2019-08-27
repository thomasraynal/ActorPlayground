using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Stopping : SystemMessage
    {
        public static Stopping Instance => new Stopping();
    }
}
