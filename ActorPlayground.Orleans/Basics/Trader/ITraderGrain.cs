using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface ITraderGrain<TEvent> : IGrainWithGuidKey, ICanSubscribe<TEvent> where TEvent : IHasStreamId
    {
        Task<IEnumerable<CcyPairChanged>> GetConsumedEvents();
    }
}
