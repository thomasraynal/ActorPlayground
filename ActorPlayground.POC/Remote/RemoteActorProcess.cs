using ActorPlayground.Remote;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Remote
{
    public class RemoteActorProcess : ActorProcess, IDisposable
    {
        private readonly Server _server;

        public RemoteActorProcess(IActorProcessConfiguration configuration, IActorRegistry registry, ISupervisorStrategy supervisionStrategy, ISerializer serializer) : base(configuration, registry, supervisionStrategy, serializer)
        {
          
            _server = new Server
            {
                Services = { Remoting.BindService(new RemoteActorService(this, registry, serializer)) },
                Ports = { new ServerPort(configuration.Uri.Host, configuration.Uri.Port, ServerCredentials.Insecure) }
            };

            _server.Start();

        }

        public void Dispose()
        {
            _server.ShutdownAsync().Wait();
        }
    }
}
