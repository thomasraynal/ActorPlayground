using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class ReaderProvider : IReaderProvider
    {
        private readonly ConcurrentDictionary<ActorId, IReader> _readerCache;
        private readonly ConcurrentDictionary<ActorId, IWriter> _writerCache;
        private readonly ISerializer _serializer;
        private readonly IActorRegistry _registry;
        private readonly IActorProcess _sender;

        public ReaderProvider(IActorProcess sender, IActorRegistry registry, ISerializer serializer)
        {
            _readerCache = new ConcurrentDictionary<ActorId, IReader>();
            _writerCache = new ConcurrentDictionary<ActorId, IWriter>();

            _serializer = serializer;
            _registry = registry;
            _sender = sender;
        }

        public IReader Get(ActorId target)
        {
            return _readerCache.GetOrAdd(target, (key) =>
            {
                switch (target.Type)
                {
                    case ActorType.Remote:
                        return null;// new RemoteReader(target, _registry, _sender);

                    default:
                        return new LocalReader(target, _registry, _sender);
                }

            });
        }
    }
}
