using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{

    public class LocalActorRegistry : IActorRegistry
    {
        private int _sequenceId;
        private readonly ISupervisorStrategy _supervisorStrategy;
        private readonly Dictionary<string, IActorProcess> _actors = new Dictionary<string, IActorProcess>();

        public LocalActorRegistry(ISupervisorStrategy supervisorStrategy, ISerializer serializer)
        {
            _supervisorStrategy = supervisorStrategy;
        }

        public IActorProcess AddTransient(Func<IActor> actorFactory, ActorType type, IActorProcess parent)
        {
            return Add(actorFactory, string.Empty, type, parent);
        }

        public IActorProcess Add(Func<IActor> actorFactory, string adress, ActorType type, IActorProcess parent)
        {
            var id = NextId(adress, type);

            var process = new ActorProcess(id, actorFactory, parent, this, _supervisorStrategy);

            process.Start();

            _actors.Add(id.Value, process);

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
        private ActorId NextId(string adress, ActorType type)
        {
            var counter = Interlocked.Increment(ref _sequenceId);
            var id = "$" + counter;

            return new ActorId(id, adress, type);
        }

    }
}
