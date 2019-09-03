using ActorPlayground.POC;
using ActorPlayground.POC.Remote;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class RemoteWriterProvider: IRemoteWriterProvider
    {
        private readonly ConcurrentDictionary<ActorId, IWriter> _writerCache;
        private readonly ISerializer _serializer;

        public RemoteWriterProvider(ISerializer serializer)
        {
            _writerCache = new ConcurrentDictionary<ActorId, IWriter>();
            _serializer = serializer;
        }

        public IWriter Get(ActorId remote, ICanPost sender)
        {
            return _writerCache.GetOrAdd(remote, (key) =>
            {
                return new RemoteWriterService(remote, sender, _serializer);

            });
        }
    }
}
