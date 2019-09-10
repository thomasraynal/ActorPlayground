using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class Subscription<TEvent>
    {
        public StreamSubscriptionHandle<TEvent> Handle { get; }
        public String Subject { get; }
        public String Provider { get; }
    }
}
