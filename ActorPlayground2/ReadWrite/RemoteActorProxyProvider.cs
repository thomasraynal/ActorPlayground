using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class RemoteActorProxyProvider : IRemoteActorProxyProvider
    {
        private readonly ConcurrentDictionary<string, ICanPost> _proxyCache;
        private readonly ISerializer _serializer;

        public RemoteActorProxyProvider(ISerializer serializer)
        {
            _proxyCache = new ConcurrentDictionary<string, ICanPost>();
            _serializer = serializer;
        }

        public ICanPost Get(string id)
        {
            return _proxyCache.GetOrAdd(id, (_) =>
            {
                return new RemoteActorProcessProxy(new ActorId(id, id, ActorType.Remote), _serializer);
            });
        }
    }
}
