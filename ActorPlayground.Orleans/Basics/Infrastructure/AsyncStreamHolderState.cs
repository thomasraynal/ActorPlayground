using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public class AsyncStreamHolderState<TEvent>
    {
        public AsyncStreamHolderState()
        {
            StreamHandles = new Dictionary<string, StreamSubscriptionHandle<TEvent>>();
        }

        public string Provider { get; set; }
        public Dictionary<string, StreamSubscriptionHandle<TEvent>> StreamHandles { get; }
    }
}
