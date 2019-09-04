using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ActorPlayground.Remote;
using Google.Protobuf;
using Grpc.Core;
using static ActorPlayground.Remote.Transport;

namespace ActorPlayground.POC.Remote
{
    public class RemoteReaderService : TransportBase
    {
        private readonly ISerializer _serializer;
        private readonly IActorProcess _self;
        private readonly IActorRegistry _registry;

        public static readonly Unit Unit = new Unit();

        public RemoteReaderService(IActorProcess self, ISerializer serializer, IActorRegistry registry)
        {
            _serializer = serializer;
            _self = self;
            _registry = registry;
        }

        private MessageEnvelope Serialize(IMessage message, ICanPost sender)
        {
            return new MessageEnvelope()
            {
                MessageData = ByteString.FromStream(new MemoryStream(_serializer.Serialize(message))),
                MessageType = message.GetType().ToString(),
                Sender = sender?.Id.ToPid()
            };
        }

        private (IMessage message, ICanPost sender) Deserialize(MessageEnvelope envelope)
        {
            var message = _serializer.Deserialize(envelope.MessageData.ToByteArray(), Type.GetType(envelope.MessageType)) as IMessage;
            var sender = envelope.Sender == null ? null : _registry.Get(envelope.Sender.Address);

            return (message, sender);

        }

        public override Task<MessageEnvelope> Send(MessageEnvelope request, ServerCallContext context)
        {
            var message = _serializer.Deserialize(request.MessageData.ToByteArray(), Type.GetType(request.MessageType)) as IMessage;

            return base.Send(request, context);
        }

        public override Task<Unit> Emit(MessageEnvelope request, ServerCallContext context)
        {
            var (message, sender) = Deserialize(request);

            _self.Post(message, sender);

            return Task.FromResult(Unit);
        }
    }
}
