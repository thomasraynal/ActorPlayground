using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface ICcyPairGrain : IGrainWithStringKey
    {
        Task Activate();
        Task Desactivate();
        Task Tick(string market, double ask, double bid);
        Task<bool> GetIsActive();
        Task<(double bid, double ask)> GetCurrentTick();
        Task<IEnumerable<IEvent>> GetAppliedEvents();
    }
}
