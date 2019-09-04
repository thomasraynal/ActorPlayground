using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ISystemEvent : IEvent
    {
        string Who { get; }
    }
}
