using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface ITraderGrain : IGrainWithGuidKey, ICanConnect
    {
        Task<IEnumerable<CcyPairChanged>> GetConsumedEvents();
    }
}
