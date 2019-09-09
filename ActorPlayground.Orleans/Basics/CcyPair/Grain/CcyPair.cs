using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{

    public class CcyPair : AggregateBase, ICcyPair
    {

        public bool IsActive { get; private set; }

        public double Ask { get; private set; }

        public double Bid { get; private set; }

        protected override void HandleEvent(IEvent @event)
        {
            switch (@event)
            {
                case ChangeCcyPairPrice changeCcyPair:
                    Ask = changeCcyPair.Ask;
                    Bid = changeCcyPair.Bid;
                    break;
                case DesactivateCcyPair _:
                    IsActive = false;
                    break;
                case ActivateCcyPair _:
                    IsActive = true;
                    break;
            }
        }
    }
}
