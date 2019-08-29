using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ActorPlayground.POC
{
    public class ActorRegistry : IActorRegistry
    {
        private int _sequenceId;
        private ICluster _cluster;
        private readonly ISupervisor _supervisor;
        private readonly Dictionary<string, ActorProcess> _actors = new Dictionary<string, ActorProcess>();

        public ActorRegistry(ISupervisor supervisor)
        {
            _supervisor = supervisor;
        }

        public void Initialize(ICluster cluster)
        {
            _cluster = cluster;
        }

        public ActorProcess Add(Func<IActor> actorFactory, ActorProcess parent)
        {
            var id = NextId();

            var process = new ActorProcess(this);
            var mailbox = new Mailbox(process, _supervisor);

            process.Initialize(id, actorFactory, mailbox, parent);

            _actors.Add(id, process);

            return process;
        }

        public ActorProcess Get(string id)
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
