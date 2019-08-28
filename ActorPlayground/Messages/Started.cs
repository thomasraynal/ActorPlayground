using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class Started : SystemMessage
    {
        public static Started Instance => new Started();
    }
}
