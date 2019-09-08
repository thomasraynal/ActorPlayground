using ActorPlayground.Explorations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public class EmptyActor : IActor
    {
        public static EmptyActor Instance => new EmptyActor();

        public Task Receive(IMessageContext context)
        {
            return Task.CompletedTask;
        }
    }
}
