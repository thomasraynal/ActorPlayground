using ActorPlayground.POC.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class RemoteReader : IReader, IDisposable
    {
        private readonly RemoteReaderEndpoint _reader;

        public RemoteReader(IActorProcess actorProcess, ISerializer serializer)
        {
            _reader = new RemoteReaderEndpoint(actorProcess, serializer);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public void Post(IMessage message)
        {
            throw new InvalidOperationException("remote reader should not be called");
        }
    }
}
