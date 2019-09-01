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

        public RemoteReaderService(IActorProcess self, ISerializer serializer)
        {
            _serializer = serializer;
            _self = self;
        }

        //public override  

        //public override Task<Unit> Receive(MessageEnvelope request, ServerCallContext context)
        //{
        //    var message = _serializer.Deserialize(request.MessageData.ToByteArray(), Type.GetType(request.MessageType)) as IMessage;

        //    //return _self.Post(message, request.Sender.ToActorId());

        //    return Task.FromResult(new Unit());
        //}
    }
}
