using Orleans.EventSourcing;
using Orleans.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    [LogConsistencyProvider(ProviderName = "CcyPairEventStore")]
    [StorageProvider(ProviderName = "CcyPairStorage")]
    public class CcyPairGrain : JournaledGrain<CcyPair>, ICcyPairGrain
    {
        
        public async Task Activate()
        {
            await ConfirmEvents();

            RaiseEvent(new ActivateCcyPair(this.IdentityString));
        }

        public async Task Desactivate()
        {
            await ConfirmEvents();

            RaiseEvent(new DesactivateCcyPair(this.IdentityString));

        }

        public async Task<IEnumerable<IEvent>> GetAppliedEvents()
        {
            await ConfirmEvents();

            var events = await RetrieveConfirmedEvents(0, Version);

            return events.Cast<IEvent>();
        }

        public Task<(double bid, double ask)> GetCurrentTick()
        {
            return Task.FromResult<(double bid, double ask)>((State.Bid, State.Ask));
        }

        public Task<bool> GetIsActive()
        {
            return Task.FromResult(State.IsActive);
        }

        public async Task Tick(string market, double ask, double bid)
        {
            await ConfirmEvents();

            RaiseEvent(new ChangeCcyPairPrice(this.IdentityString, market, ask, bid));

        }
    }
}
