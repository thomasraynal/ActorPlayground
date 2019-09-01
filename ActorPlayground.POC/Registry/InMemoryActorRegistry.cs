using ActorPlayground.POC.Remote;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{

    public class InMemoryActorRegistry : IActor, IActorRegistry
    {
        private int _sequenceId;
        private readonly IContainer _container;
        private readonly ConcurrentDictionary<string, IActorProcess> _actors = new ConcurrentDictionary<string, IActorProcess>();

        //refacto: IActorRegistry as Actor
        public InMemoryActorRegistry(IContainer container)
        {
            _container = container;

            Add(() => this, ActorType.Registry, null);
        }

        public IActorProcess AddInternal<TActorProcess>(IActorProcessConfiguration configuration) where TActorProcess: IActorProcess
        {

            var args = new ExplicitArguments();
            args.Set(configuration);

            var process = _container.GetInstance<TActorProcess>(args);

            _actors.AddOrUpdate(configuration.Id.Value, process, (key, @new) => @new);

            process.Start();

            return process;
        }

        public IActorProcess Add(Func<IActor> actorFactory, ActorType type, IActorProcess parent)
        {
            var id = NextId(string.Empty, type);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, type);

            return AddInternal<IActorProcess>(configuration);
        }

        public IActorProcess Add(Func<IActor> actorFactory, string adress, ActorType type, IActorProcess parent)
        {
            var id = NextId(adress, type);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, type, new Uri(adress));

            return AddInternal<IActorProcess>(configuration);
        }

        public IActorProcess Get(string id)
        {
            if (!_actors.ContainsKey(id)) throw new Exception("not exist");

            return _actors[id];
        }

        //refacto: remove all children
        public void Remove(string id)
        {
            if (!_actors.ContainsKey(id)) throw new Exception("not exist");

            _actors.Remove(id, out var _);

        }

        //refacto : id provider
        private ActorId NextId(string adress, ActorType type)
        {
            var counter = Interlocked.Increment(ref _sequenceId);
            var id = "$" + counter;

            return new ActorId(id, adress, type);
        }

        public void Dispose()
        {
           foreach(var actor in _actors)
            {
                actor.Value.Stop();

                if(actor.Value is IDisposable)
                {
                   ((IDisposable)actor.Value).Dispose();
                }
            }

            _actors.Clear();
        }

        public Task Receive(IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
