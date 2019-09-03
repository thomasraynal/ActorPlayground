using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ActorPlayground.Remote;
using Grpc.Core;
using static ActorPlayground.Remote.Reader;

namespace ActorPlayground.POC.Remote
{
    public class RemoteReaderService : ReaderBase
    {
        private readonly ISerializer _serializer;
        private readonly IActorProcess _self;

        public static readonly Unit Unit = new Unit();

        public RemoteReaderService(IActorProcess self, ISerializer serializer)
        {
            _serializer = serializer;
            _self = self;
        }

        public override Task<MessageEnvelope> ReceiveCommand(MessageEnvelope request, ServerCallContext context)
        {
            var message = _serializer.Deserialize(request.MessageData.ToByteArray(), Type.GetType(request.MessageType)) as IMessage;


            return Task.FromResult<MessageEnvelope>(null);
        }

        public override Task<Unit> ReceiveEvent(MessageEnvelope request, ServerCallContext context)
        {
            return Task.FromResult(Unit);
        }

 
    }
}
