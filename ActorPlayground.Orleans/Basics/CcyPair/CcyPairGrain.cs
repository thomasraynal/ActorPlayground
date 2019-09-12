using ActorPlayground.Orleans.Basics.EventStore;
using Orleans.Core;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Providers;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{

    [LogConsistencyProvider(ProviderName = "CcyPairEventStore")]
    [StorageProvider(ProviderName = "CcyPairStorage")]
    public class CcyPairGrain : EventStoreJournaledGrain<CcyPair,IEvent>, ICcyPairGrain
    {
        public CcyPairGrain(IEventStoreRepositoryConfiguration eventStoreConfiguration) : base(eventStoreConfiguration)
        {
        }

        public async Task Activate()
        {
            RaiseEvent(new ActivateCcyPair(this.IdentityString));

             await ConfirmEvents();
        }

        public async Task Desactivate()
        {

            RaiseEvent(new DesactivateCcyPair(this.IdentityString));

            await ConfirmEvents();

        }

        //todo : retrieve events from event store
        public async Task<IEnumerable<IEvent>> GetAppliedEvents()
        {
            throw new NotImplementedException();

            //await ConfirmEvents();

            //var events = await RetrieveConfirmedEvents(0, Version);

            //return events.Cast<IEvent>();
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
   
            RaiseEvent(new ChangeCcyPairPrice(this.IdentityString, market, ask, bid));

            await ConfirmEvents();

        }
    }
}
