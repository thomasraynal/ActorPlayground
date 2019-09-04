using ActorPlayground.POC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ICandSend
    {
        Task<T> Send<T>(string targetId, IEvent message) where T : IEvent;
        Task<T> Send<T>(string targetId, IEvent message, CancellationToken cancellationToken) where T : IEvent;
        Task<T> Send<T>(string targetId, IEvent message, TimeSpan timeout) where T : IEvent;
    }
}
