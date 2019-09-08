using ActorPlayground.Remote;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations.Remote
{
    public class RemoteWriterService : IWriter, IDisposable
    {

        private readonly ISerializer _serializer;
        private readonly Channel _channel;
        private readonly Transport.TransportClient _client;

        public RemoteWriterService(ICanPost remote, ISerializer serializer)
        {
            var uri = new Uri(remote.Id.Adress);

            _channel = new Channel($"{uri.Host}:{uri.Port}", ChannelCredentials.Insecure);
            _client = new Transport.TransportClient(_channel);
            _serializer = serializer;

        }

        public void Dispose()
        {
            _channel.ShutdownAsync().Wait();
        }

        private MessageEnvelope Serialize(IEvent message, ICanPost sender)
        {
            return new MessageEnvelope()
            {
                MessageData = ByteString.FromStream(new MemoryStream(_serializer.Serialize(message))),
                MessageType = message.GetType().ToString(),
                Sender = sender?.Id.ToPid()
            };
        }

        public void Write(IEvent message, ICanPost sender)
        {
            _client.Send(Serialize(message, sender));
        }

        public void Write(ICommand message, ICanPost sender, CancellationToken cancellationToken)
        {
            _client.Send(Serialize(message, sender), cancellationToken: cancellationToken);
        }

        public void Write(ICommand message, ICanPost sender, TimeSpan timeout)
        {
            _client.Send(Serialize(message, sender), deadline: DateTime.Now.Add(timeout));
        }
    }
}
