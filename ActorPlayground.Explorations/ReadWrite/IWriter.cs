using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface IWriter
    {
        void Write(IEvent message, ICanPost sender);
        void Write(ICommand message, ICanPost sender, CancellationToken cancellationToken);
        void Write(ICommand message, ICanPost sender, TimeSpan timeout);
    }
}
