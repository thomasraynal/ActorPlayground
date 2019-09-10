using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public interface IConsumer<TEvent> :  ICanObserve<TEvent> where TEvent : IHasStreamId
    {
    }
}
