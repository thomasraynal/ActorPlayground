using System;
using EventStore.ClientAPI;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IConnectionStatusMonitor
    {
        IObservable<bool> IsConnected { get; }
        IObservable<IConnected<IEventStoreConnection>> GetEventStoreConnectedStream();
    }
}