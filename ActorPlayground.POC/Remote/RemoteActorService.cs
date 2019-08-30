using ActorPlayground.Remote;
using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC.Remote
{
    public class RemoteActorService : Remoting.RemotingBase
    {
        private readonly IActorProcess _actorProcess;
        private readonly IActorRegistry _registry;
        private readonly ISerializer _serializer;

        private static readonly ConnectResponse ConnectResponse = new ConnectResponse();

        public RemoteActorService(IActorProcess actorProcess, IActorRegistry registry, ISerializer serializer)
        {
            _actorProcess = actorProcess;
            _registry = registry;
            _serializer = serializer;
        }

        public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            return Task.FromResult(ConnectResponse);
        }

        private void ProcessMessage(MessageEnvelope envelop)
        {
            var message = _serializer.Deserialize(envelop.MessageData.ToByteArray(), Type.GetType(envelop.MessageType));
            _actorProcess.Post(message as IMessage, null);
        }

        public async override Task Receive(IAsyncStreamReader<MessageBatch> requestStream, IServerStreamWriter<Unit> responseStream, ServerCallContext context)
        {
            //refacto : handle response
            await requestStream.ForEachAsync( batch =>
            {
                foreach (var envelop in batch.Envelopes)
                {
                    ProcessMessage(envelop);
                }

                return Task.CompletedTask;

            });

        }

        public override Task<MessageEnvelope> Send(MessageEnvelope request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

    }
}
