using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Supervisor : ISupervisor
    {
        private readonly ISupervisorStrategy _supervisorStrategy;

        public Supervisor(ISupervisorStrategy strategy)
        {
            _supervisorStrategy = strategy;
        }

        public async Task Receive(IContext context)
        {
            var message = context.Message as ISystemMessage;

            if (null == message) return;

            await _supervisorStrategy.Handle(message);
        }
    }
}
