using ActorPlayground.Remote;
using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.Threading.Tasks;

namespace ActorPlayground.POC.Remote
{
    public class RemoteWriterService : Writer.WriterBase
    {
        private readonly IActorProcess _actorProcess;
        private readonly IActorRegistry _registry;
        private readonly ISerializer _serializer;

        public RemoteWriterService(IActorProcess actorProcess, ISerializer serializer)
        {
            _actorProcess = actorProcess;
            _serializer = serializer;
        }

        public override Task<MessageEnvelope> Send(MessageEnvelope request, ServerCallContext context)
        {
            var message = _serializer.Deserialize(request.MessageData.ToByteArray(), Type.GetType(request.MessageType)) as IMessage;

            return null;

        }

        public override Task<Unit> Emit(MessageEnvelope request, ServerCallContext context)
        {
            return base.Emit(request, context);
        }

    }
}
