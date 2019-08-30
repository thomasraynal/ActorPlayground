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

        public RemoteActorProcess(ActorId id, Func<IActor> actorFactory, IActorProcess parent, IActorRegistry registry, ISupervisorStrategy supervisionStrategy, ISerializer serializer) : base(id, actorFactory, parent, registry, supervisionStrategy)
        {
            var uri = new Uri(id.Value);

            _server = new Server
            {
                Services = { Remoting.BindService(new RemoteActorService(this, registry, serializer)) },
                Ports = { new ServerPort(uri.Host, uri.Port, ServerCredentials.Insecure) }
            };

            _server.Start();

        }

        public void Dispose()
        {
            _server.ShutdownAsync().Wait();
        }
    }
}
