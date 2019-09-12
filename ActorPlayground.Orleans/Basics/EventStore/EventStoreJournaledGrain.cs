using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using Orleans;
using Orleans.Core;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public abstract class EventStoreJournaledGrain<TState, TEvent> : JournaledGrain<TState, TEvent>, ICustomStorageInterface<TState, TEvent>
        where TState : class, IAggregate, new()
        where TEvent : class, IEvent
    {

        private readonly IEventStoreRepositoryConfiguration _eventStoreConfiguration;
        private EventStoreRepository _repository;

        protected EventStoreJournaledGrain(IEventStoreRepositoryConfiguration eventStoreConfiguration)
        {
            _eventStoreConfiguration = eventStoreConfiguration;
        }

        public override Task OnActivateAsync()
        {
            _repository = EventStoreRepository.Create(_eventStoreConfiguration);
            return Task.CompletedTask;
        }

        public override Task OnDeactivateAsync()
        {
            _repository.Dispose();

            return Task.CompletedTask;
        }

        public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<TEvent> updates, int expectedversion)
        {
            try
            {
                await _repository.Save<TState>(IdentityString, Version - 1, updates.Cast<IEvent>());
            }
            //https://dotnet.github.io/orleans/Documentation/grains/event_sourcing/log_consistency_providers.html
            catch (WrongExpectedVersionException)
            {
                return false;
            }

            return true;
        }

        public async Task<KeyValuePair<int, TState>> ReadStateFromStorage()
        {
            var (version, state) = await _repository.GetById<string, TState>(IdentityString);

            return KeyValuePair.Create(version, state);
        }
    }
}
