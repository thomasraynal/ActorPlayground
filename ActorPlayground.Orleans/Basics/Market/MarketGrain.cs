using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public class MarketGrain : Producer<CcyPairChanged>, IMarketGrain
    {
        public Task OnTick(string id, double bid, double ask)
        {
            return OnNext(new CcyPairChanged(id, true, ask, bid));
        }
    }
}
