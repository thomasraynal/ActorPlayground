using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{

    [StorageProvider(ProviderName = "AsyncStreamHandlerStorage")]
    public class FxFeedGrain : Consumer<CcyPairChanged>, IFxFeedGrain<CcyPairChanged>
    {
        public List<CcyPairChanged> _consumedEvents;

        public FxFeedGrain()
        {
            _consumedEvents = new List<CcyPairChanged>();
        }

        public Task Desactivate()
        {
            DeactivateOnIdle();

            return Task.CompletedTask;
        }

        public Task<IEnumerable<CcyPairChanged>> GetConsumedEvents()
        {
            return Task.FromResult(_consumedEvents.AsEnumerable());
        }

        public override Task OnNextAsync(CcyPairChanged @event, StreamSequenceToken token)
        {
            _consumedEvents.Add(@event);

            return Task.CompletedTask;
        }

    }
}
