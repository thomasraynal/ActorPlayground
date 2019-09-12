using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IConnectionStatusMonitor : IDisposable
    {
        bool IsConnected { get; }
        Task Connect();
    }
}