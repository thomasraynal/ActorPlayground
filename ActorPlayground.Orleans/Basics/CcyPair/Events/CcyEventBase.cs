using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public abstract class CcyEventBase : IEvent
    {
        protected CcyEventBase()
        {
        }

        protected CcyEventBase(string ccyPair)
        {
            StreamId = ccyPair;
        }

        public string StreamId { get; internal set; }
    }
}
