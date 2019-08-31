using ActorPlayground.POC.Message;
using StructureMap;
using StructureMap.Pipeline;
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
        private readonly ISerializer _serializer;
        private readonly IContainer _container;
        private readonly Dictionary<string, IActorProcess> _actors = new Dictionary<string, IActorProcess>();

        public LocalActorRegistry(IContainer container, ISupervisorStrategy supervisorStrategy, ISerializer serializer)
        {
            _supervisorStrategy = supervisorStrategy;
            _serializer = serializer;
            _container = container;
        }

        public IActorProcess AddTransient(Func<IActor> actorFactory, ActorType type, IActorProcess parent)
        {
            return Add(actorFactory, string.Empty, type, parent);
        }

        public IActorProcess Add(Func<IActor> actorFactory, string adress, ActorType type, IActorProcess parent)
        {
            var id = NextId(adress, type);

            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, type);

            var args = new ExplicitArguments();
            args.Set<IActorProcessConfiguration>(configuration);

            var process = _container.GetInstance<IActorProcess>(args);

            _actors.Add(id.Value, process);

            process.Start();

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
