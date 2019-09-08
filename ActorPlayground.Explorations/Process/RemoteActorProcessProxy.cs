using ActorPlayground.Explorations.Remote;

namespace ActorPlayground.Explorations
{
    public class RemoteActorProcessProxy : ICanPost
    {
        private readonly ActorId _remote;
        private readonly RemoteWriterService _writer;

        public RemoteActorProcessProxy(ActorId remote, ISerializer serializer)
        {
            _remote = remote;
            _writer = new RemoteWriterService(this, serializer);
        }

        public ActorId Id => _remote;

        public void Post(IEvent msg, ICanPost sender)
        {
            _writer.Write(msg, sender);
        }
    }
}
