using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Stopped : SystemMessage
    {
        public static Stopped Instance => new Stopped();
    }
}
