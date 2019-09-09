using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class DesactivateCcyPair : CcyEventBase
    {
        public DesactivateCcyPair(string ccyPair) : base(ccyPair)
        {
        }
    }
}
