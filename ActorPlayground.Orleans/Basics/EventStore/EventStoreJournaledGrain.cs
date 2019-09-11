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
        private IEventStoreConnection _eventStoreConnection;
        private EventStoreRepository _repository;

        public abstract IEventStoreRepositoryConfiguration EventStoreConfiguration { get; }

        public override Task OnActivateAsync()
        {
            _eventStoreConnection = new ExternalEventStore(EventStoreConfiguration.ConnectionString, EventStoreConfiguration.ConnectionSettings).Connection;

            _repository = new EventStoreRepository(EventStoreConfiguration, _eventStoreConnection, new ConnectionStatusMonitor(_eventStoreConnection));

            return Task.CompletedTask;
        }

        public override Task OnDeactivateAsync()
        {
            _eventStoreConnection.Close();
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
            catch (WrongExpectedVersionException ex)
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
