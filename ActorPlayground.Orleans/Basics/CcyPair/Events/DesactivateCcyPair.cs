using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    [Serializable]
    public class DesactivateCcyPair : CcyEventBase
    {
        public DesactivateCcyPair()
        {
        }

        public DesactivateCcyPair(string ccyPair) : base(ccyPair)
        {
        }
    }
}
