using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{

    public class ActorRegistry : IActorRegistry
    {
        private int _sequenceId;
        private readonly ISupervisorStrategy _supervisorStrategy;
        private readonly Dictionary<string, IActorProcess> _actors = new Dictionary<string, IActorProcess>();

        public ActorRegistry(ISupervisorStrategy supervisorStrategy)
        {
            _supervisorStrategy = supervisorStrategy;
        }

        public IActorProcess Add(Func<IActor> actorFactory, IActorProcess parent)
        {
            var id = NextId();

            var process = new ActorProcess(id, actorFactory, parent, this, _supervisorStrategy);

            process.Start();

            _actors.Add(id, process);

            return process;
        }

        public IActorProcess Get(string id)
        {
            if (!_actors.ContainsKey(id)) throw new Exception("not exist");

            return _actors[id];
        }

        public void Remove(string id)
        {
            if (!_actors.ContainsKey(id)) throw new Exception("not exist");

            _actors.Remove(id);

        }

        public string NextId()
        {
            var counter = Interlocked.Increment(ref _sequenceId);
            return "$" + counter;
        }

    }
}
