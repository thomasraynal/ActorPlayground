using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
   public class Restarting : SystemMessage
    {
        public static Restarting Instance => new Restarting();
    }
}
