using ActorPlayground.Remote;
using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC.Remote
{
    public class RemoteWriterService : IWriter, IDisposable
    {
        private readonly ActorId _remote;
        private readonly ICanPost _sender;
        private readonly ISerializer _serializer;
        private readonly Channel _channel;
        private readonly Writer.WriterClient _client;

        public RemoteWriterService(ActorId remote, ICanPost sender, ISerializer serializer)
        {
            _remote = remote;
            _sender = sender;
            _serializer = serializer;

            _channel = new Channel(remote.Adress, ChannelCredentials.Insecure);
            _client = new Writer.WriterClient(_channel);
        }

        public void Dispose()
        {
            _channel.ShutdownAsync().Wait();
        }

        private MessageEnvelope Serialize(IMessage message)
        {
            return new MessageEnvelope()
            {
              
            };
        }

        public void Emit(IMessage message)
        {
            _client.Emit(Serialize(message));
        }

        public void Emit(ICanPost target, IMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<T> Send<T>(ICanPost target, IMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<T> Send<T>(ICanPost target, IMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> Send<T>(ICanPost target, IMessage message, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}
