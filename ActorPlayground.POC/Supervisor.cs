using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Supervisor : ISupervisor
    {
        private readonly ISupervisorStrategy _supervisorStrategy;
        private IActorRegistry _registry;
        private ActorProcess _process;

        public ActorProcess Process { get; }

        public Supervisor(ISupervisorStrategy strategy)
        {
            _supervisorStrategy = strategy;
        }

        public void Initialize(IActorRegistry registry)
        {
            _registry = registry;
            _process = _registry.Add(() => this, null);
        }

        public async Task Receive(IContext context)
        {
            var message = context.Message as ISystemMessage;

            if (null == message) return;

            await _supervisorStrategy.Handle(message);
        }
    }
}
