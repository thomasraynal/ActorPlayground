using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class ClusterRegistry : IClusterRegistry
    {
        private readonly IDirectory _directory;
        private readonly InMemoryActorRegistry _registry;

        public ClusterRegistry(IContainer container, IDirectory directory, IRemoteActorProxyProvider remoteActorProxyProvider)
        {
            _directory = directory;
            _registry = new InMemoryActorRegistry(container, remoteActorProxyProvider);
        }

        public IActorProcess Add(Func<IActor> actorFactory,  ICanPost parent)
        {
            return _registry.Add(actorFactory, parent);
        }

        public IActorProcess Add(Func<IActor> actorFactory, string adress, ICanPost parent)
        {
            var process = _registry.Add(actorFactory, adress, parent);

            _directory.Register(new ClusterMember(process.Id));

            return process;
        }

        public void Dispose()
        {
            _registry.Dispose();
        }

        public ICanPost Get(string id)
        {
            return _registry.Get(id);
        }

        public Task Receive(IMessageContext context)
        {
            return _registry.Receive(context);
        }

        public void Remove(string id)
        {
            _directory.Unregister(id);
            _registry.Remove(id);
        }
    }
}
