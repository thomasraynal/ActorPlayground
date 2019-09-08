using ActorPlayground.Explorations.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public class RemoteActorProcess : ActorProcess, IRemoteActorProcess, IDisposable
    {
        private readonly RemoteReaderEndpoint _remoteReaderEndpoint;

        public RemoteActorProcess(IActorProcessConfiguration configuration, IActorRegistry registry, ISupervisorStrategy supervisionStrategy, ISerializer serializer) : base(configuration, registry, supervisionStrategy)
        {
            _remoteReaderEndpoint = new RemoteReaderEndpoint(this, serializer, registry);
        }

        public void Dispose()
        {
            _remoteReaderEndpoint.Dispose();
        }
    }
}
