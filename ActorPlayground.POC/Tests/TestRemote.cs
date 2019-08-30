using ActorPlayground.Remote;
using Grpc.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class RemotingImpl : Remoting.RemotingBase
    {

        public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ConnectResponse() { Ack = true });
        }

        public override Task Receive(IAsyncStreamReader<MessageBatch> requestStream, IServerStreamWriter<Unit> responseStream, ServerCallContext context)
        {
            return base.Receive(requestStream, responseStream, context);
        }

        public override Task<MessageEnvelope> Send(MessageEnvelope request, ServerCallContext context)
        {
            return base.Send(request, context);
        }

    }

    [TestFixture]
    public class TestRemote
    {

        [Test]
        public async Task Test()
        {

            Task.Run(() =>
            {

                Server server = new Server
                {
                    Services = { Remoting.BindService(new RemotingImpl()) },
                    Ports = { new ServerPort("localhost", 8080, ServerCredentials.Insecure) }
                };

                server.Start();

   
                Console.ReadKey();

                server.ShutdownAsync().Wait();
            });


            await Task.Delay(1000);

                Task.Run(() =>
            {

                try
                {
                    var channel = new Channel("localhost:8080", ChannelCredentials.Insecure);
                    var client = new Remoting.RemotingClient(channel);
                    var result = client.Connect(new ConnectRequest() { Sender = new PID() { Address = "adresse", Id = "id" } });

                    channel.ShutdownAsync().Wait();

                }
                catch (Exception ex)
                {

                }

            });

          await  Task.Delay(1000);



        
        }
    }
}
