using Orleans.EventSourcing;
using Orleans.Providers;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    [StorageProvider(ProviderName = "CcyPairStorage")]
    public class CcyPairGrain : JournaledGrain<CcyPair>, ICcyPairGrain
    {
        
        public Task Activate()
        {
            RaiseEvent(new ActivateCcyPair(this.IdentityString));

            return Task.CompletedTask;
        }

        public Task Desactivate()
        {
            RaiseEvent(new DesactivateCcyPair(this.IdentityString));

            return Task.CompletedTask;
        }

        public Task<(double bid, double ask)> GetCurrentTick()
        {
            return Task.FromResult<(double bid, double ask)>((State.Bid, State.Ask));
        }

        public Task<bool> GetIsActive()
        {
            return Task.FromResult(State.IsActive);
        }

        public Task Tick(string market, double ask, double bid)
        {
            RaiseEvent(new ChangeCcyPairPrice(this.IdentityString, market, ask, bid));

            return Task.CompletedTask;
        }
    }
}
