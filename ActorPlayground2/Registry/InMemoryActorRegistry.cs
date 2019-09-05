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

        public IActorProcess Add(Func<IActor> actorFactory, ActorType type, ICanPost parent)
        {
            var id = NextId(null, type);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, type);

            return AddInternal<IActorProcess>(configuration);
        }

        public IActorProcess Add(Func<IActor> actorFactory, string adress, ActorType type, ICanPost parent)
        {
            var id = NextId(adress, type);
            var configuration = new ActorProcessConfiguration(id, actorFactory, parent, type, new Uri(adress));

            return AddInternal<IRemoteActorProcess>(configuration);
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
        public void Remove(string id)
        {
            if (!_actors.ContainsKey(id)) throw new Exception("not exist");

            _actors.Remove(id, out var _);

        }

        //refacto : id provider
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
