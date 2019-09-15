using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    //todo : handle persistent subscriptions
    public interface IEventStoreRepository
    {
        bool IsConnected { get; }
        Task Connect(TimeSpan timeout);
        bool IStarted { get; }
        Task<(int version, TAggregate aggregate)> GetAggregate<TKey, TAggregate>(TKey id) where TAggregate : class, IAggregate, new();
        IObservable<IEvent> Observe(string streamId, long? fromIncluding = null, bool rewindAfterDisconnection = false);
        IObservable<IEvent> ObservePersistentSubscription(string streamId, string group)
        Task SavePendingEvents(string streamId, long originalVersion, IEnumerable<IEvent> pendingEvents, params KeyValuePair<string, string>[] extraHeaders);
    }
}