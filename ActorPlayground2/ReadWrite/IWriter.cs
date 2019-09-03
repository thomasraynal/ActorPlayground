using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IWriter
    {
        void Emit(IMessage message);
        void Emit(ICanPost target, IMessage message);
        Task<T> Send<T>(ICanPost target, IMessage message);
        Task<T> Send<T>(ICanPost target, IMessage message, CancellationToken cancellationToken);
        Task<T> Send<T>(ICanPost target, IMessage message, TimeSpan timeout);
    }
}
