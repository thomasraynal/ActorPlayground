using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface IFxFeedGrain<TEvent> : IGrainWithGuidKey, ICanConnect, ICanSubscribe where TEvent : IEvent
    {
        Task<IEnumerable<TEvent>> GetConsumedEvents();
        Task Desactivate();
    }
}
