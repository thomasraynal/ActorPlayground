using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{

    public class InMemoryActorRegistry : IActorRegistry
    {
        private int _sequenceId;
        private readonly IContainer _container;
        private readonly IRemoteActorProxyProvider _remoteActorProxyProvider;
        private readonly string _prefix;
        private readonly ConcurrentDictionary<string, IActorProcess> _actors = new ConcurrentDictionary<string, IActorProcess>();

        //refacto: IActorRegistry as Actor
        public InMemoryActorRegistry(IContainer container, IRemoteActorProxyProvider remoteActorProxyProvider)
        {
            _container = container;
            _remoteActorProxyProvider = remoteActorProxyProvider;

            //refacto: config
            _prefix = DateTime.Now.Ticks.ToString();

            //   Add(() => this, ActorType.Registry, null);
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

        public IActorProcess Add(Func<IActor> actorFactory, ICanPost parent)
        {
            var id = NextId(null, ActorType.Transient);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, id.Type);

            return AddInternal<IActorProcess>(configuration);
        }

        public IActorProcess Add(Func<IActor> actorFactory, string adress, ICanPost parent)
        {
            var id = NextId(adress, ActorType.Remote);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, id.Type, new Uri(adress));

            return AddInternal<IRemoteActorProcess>(configuration);
        }

        public IActorProcess Add(Func<IActor> actorFactory, ICanPost parent, string name)
        {
            if (_actors.ContainsKey(name)) throw new Exception("already exist");

            var id = NextId(name, null, ActorType.Remote);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, id.Type);

            return AddInternal<IActorProcess>(configuration);
        }

        public IActorProcess Add(Func<IActor> actorFactory, string adress, ICanPost parent, string name)
        {
            if (_actors.ContainsKey(name)) throw new Exception("already exist");

            var id = NextId(name, adress, ActorType.Remote);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, id.Type);

            return AddInternal<IActorProcess>(configuration);
        }

        public ICanPost Get(string id)
        {

            if (!_actors.ContainsKey(id))
            {
                var canCreate = Uri.TryCreate(id, UriKind.Absolute, out var uriResult);

                if (canCreate && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                {
                   return _remoteActorProxyProvider.Get(id);
                }
                else
                {
                    throw new Exception("not exist");
                }
            }


            return _actors[id];
        }

        //refacto: remove all children
        public void Remove(ActorId id)
        {
            if (!_actors.ContainsKey(id.Value)) throw new Exception("not exist");

            _actors.Remove(id.Value, out var _);

        }

        //refacto : id provider
        private ActorId NextId(string name, string adress, ActorType type)
        {
            return new ActorId(name, adress, type);
        }

        private ActorId NextId(string adress, ActorType type)
        {

            var counter = Interlocked.Increment(ref _sequenceId);
            var id = _prefix + "$" + counter;

            return new ActorId(id, adress, type);
        }

        public void Dispose()
        {
           foreach(var actor in _actors)
            {
                actor.Value.Stop();

                if(actor.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            _actors.Clear();
        }

        public Task Receive(IMessageContext context)
        {
            throw new NotImplementedException();
        }


    }
}
