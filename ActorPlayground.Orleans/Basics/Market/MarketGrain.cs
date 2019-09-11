using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public class MarketGrain : Producer<CcyPairChanged>, IMarketGrain
    {
        public Task Tick(string ccyPair, double bid, double ask)
        {
            return Next(new CcyPairChanged(this.IdentityString, ccyPair, true, ask, bid));
        }
    }
}
