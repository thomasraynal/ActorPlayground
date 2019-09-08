using ActorPlayground.Remote;
using Grpc.Core;
using System;

namespace ActorPlayground.Explorations.Remote
{
    public class RemoteReaderEndpoint : IDisposable
    {
        private readonly Server _server;

        public RemoteReaderEndpoint(IActorProcess process, ISerializer serializer, IActorRegistry registry)
        {

            _server = new Server
            {
                Services = { Transport.BindService(new RemoteReaderService(process, serializer, registry)) },
                Ports = { new ServerPort(process.Configuration.Uri.Host, process.Configuration.Uri.Port, ServerCredentials.Insecure) }
            };

            _server.Start();
        }

        public void Dispose()
        {
            _server.ShutdownAsync().Wait();
        }
    }
}
