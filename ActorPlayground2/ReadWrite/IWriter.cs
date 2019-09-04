using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IWriter
    {
        void Emit(IMessage message, ICanPost sender);
        Task Send<T>(IMessage message, ICanPost sender);
        Task Send<T>(IMessage message, ICanPost sender, CancellationToken cancellationToken);
        Task Send<T>(IMessage message, ICanPost sender, TimeSpan timeout);
    }
}
