using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    internal class EmptyActor : IActor
    {
        private readonly Receive _receive;
        public EmptyActor(Receive receive) => _receive = receive;
        public Task ReceiveAsync(IContext context) => _receive(context);
    }
}
