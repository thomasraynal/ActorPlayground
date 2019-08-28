using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public delegate Task Receive(IContext context);

    ////TODO: IReceiveContext ?
    public delegate Task Receiver(IReceiverContext context, MessageEnvelope envelope);

    public delegate Task Sender(ISenderContext context, PID target, MessageEnvelope envelope);
}
