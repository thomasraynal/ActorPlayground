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
        Task<T> Send<T>(string targetId, ICommand message) where T : ICommandResult;
        Task<T> Send<T>(string targetId, ICommand message, CancellationToken cancellationToken) where T : ICommandResult;
        Task<T> Send<T>(string targetId, ICommand message, TimeSpan timeout) where T : ICommandResult;
    }
}
