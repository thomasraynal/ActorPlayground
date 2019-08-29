using System;
using System.Collections.Generic;
using System.Threading;

namespace ActorPlayground.POC
{
    public class ActorRegistry : IActorRegistry
    {
        private int _sequenceId;
        private Dictionary<string, ActorProcess> _actors = new Dictionary<string, ActorProcess>();

        public ActorProcess Add(IActor actor)
        {
            var id = NextId();
            var process = new ActorProcess(id, actor);
            _actors.Add(id, process);

            return process;
        }

        public ActorProcess Get(string id)
        {
            if (!_actors.ContainsKey(id)) throw new Exception("not exist");

            return _actors[id];
        }

        public void Remove(ActorProcess actor)
        {
            _actors.Remove(actor.Id);
        }

        public string NextId()
        {
            var counter = Interlocked.Increment(ref _sequenceId);
            return "$" + counter;
        }

    }
}
