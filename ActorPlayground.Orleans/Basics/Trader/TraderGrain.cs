using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{

    public class TraderGrain : Consumer<CcyPairChanged>, ITraderGrain<CcyPairChanged>
    {
        public List<CcyPairChanged> _consumedEvents;

        public TraderGrain()
        {
            _consumedEvents = new List<CcyPairChanged>();
        }

        public Task<IEnumerable<CcyPairChanged>> GetConsumedEvents()
        {
            return Task.FromResult(_consumedEvents.AsEnumerable());
        }

        public override Task OnNext(CcyPairChanged @event)
        {
            _consumedEvents.Add(@event);

            return Task.CompletedTask;
        }

    }
}
