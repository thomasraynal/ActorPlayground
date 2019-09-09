using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class ActivateCcyPair : CcyEventBase
    {
        public ActivateCcyPair(string ccyPair) : base(ccyPair)
        {
        }
    }
}
