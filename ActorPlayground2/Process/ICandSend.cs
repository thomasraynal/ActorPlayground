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
        Task<T> Send<T>(string targetId, IMessage message) where T : IMessage;
        Task<T> Send<T>(string targetId, IMessage message, CancellationToken cancellationToken) where T : IMessage;
        Task<T> Send<T>(string targetId, IMessage message, TimeSpan timeout) where T : IMessage;
    }
}
