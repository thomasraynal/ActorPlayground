using ActorPlayground.POC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class DefaultRootActor : IActor
    {
        public Task Receive(IMessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}
