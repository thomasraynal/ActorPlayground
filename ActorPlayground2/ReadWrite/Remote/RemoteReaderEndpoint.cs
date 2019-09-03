using ActorPlayground.Remote;
using Grpc.Core;
using System;

namespace ActorPlayground.POC.Remote
{
    public class RemoteReaderEndpoint : IDisposable
    {
        private readonly Server _server;

        public RemoteReaderEndpoint(IActorProcess process, ISerializer serializer)
        {

            _server = new Server
            {
                Services = { Reader.BindService(new RemoteReaderService(process, serializer)) },
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
