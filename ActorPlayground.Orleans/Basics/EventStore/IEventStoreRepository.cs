﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IEventStoreRepository
    {
        bool IsConnected { get; }
        Task Connect(TimeSpan timeout);
        Task<(int version, TAggregate aggregate)> GetAggregate<TKey, TAggregate>(TKey id) where TAggregate : class, IAggregate, new();
        IObservable<IEvent> Observe(string streamId, long? fromIncluding = null, bool rewindAfterDisconnection = false);
        Task SavePendingEvents(string streamId, long originalVersion, IEnumerable<IEvent> pendingEvents, params KeyValuePair<string, string>[] extraHeaders);
    }
}