using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public interface IReceiverContext : IInfoContext
    {
        Task Receive(MessageEnvelope envelope);
    }
}
