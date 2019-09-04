using ActorPlayground.Remote;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC.Remote
{
    public class RemoteWriterService : IWriter, IDisposable
    {
        private readonly ICanPost _remote;
        private readonly ISerializer _serializer;
        private readonly Channel _channel;
        private readonly Transport.TransportClient _client;

        public RemoteWriterService(ICanPost remote, ISerializer serializer)
        {
            _remote = remote;
            _serializer = serializer;

            var uri = new Uri(remote.Id.Adress);

            _channel = new Channel($"{uri.Host}:{uri.Port}", ChannelCredentials.Insecure);
            _client = new Transport.TransportClient(_channel);

        }

        public void Dispose()
        {
            _channel.ShutdownAsync().Wait();
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

        private IMessage Deserialize(MessageEnvelope envelope)
        {
            return _serializer.Deserialize(envelope.MessageData.ToByteArray(), Type.GetType(envelope.MessageType)) as IMessage;
        }

        public void Emit(IMessage message, ICanPost sender)
        {
            _client.Emit(Serialize(message, sender));
        }

        public Task Send<T>(IMessage message, ICanPost sender)
        {
            var result = _client.Send(Serialize(message, sender));

            sender.Post(Deserialize(result), _remote);

            return Task.CompletedTask;
        }

        public Task Send<T>(IMessage message, ICanPost sender, CancellationToken cancellationToken)
        {
            var result = _client.Send(Serialize(message, sender), cancellationToken: cancellationToken);

            sender.Post(Deserialize(result), _remote);

            return Task.CompletedTask;
        }

        public Task Send<T>(IMessage message, ICanPost sender, TimeSpan timeout)
        {
            var result = _client.Send(Serialize(message, sender), deadline: DateTime.Now.Add(timeout));

            sender.Post(Deserialize(result), _remote);

            return Task.CompletedTask;
        }
    }
}
