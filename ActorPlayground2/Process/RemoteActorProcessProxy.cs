using ActorPlayground.POC.Remote;

namespace ActorPlayground.POC
{
    public class RemoteActorProcessProxy : ICanPost
    {
        private readonly RemoteWriterService _writer;

        public RemoteActorProcessProxy(ActorId remote, IActorProcess sender, ISerializer serializer)
        {
            _writer = new RemoteWriterService(remote, sender, serializer);
        }

        public void Post(IMessage msg, ICanPost sender)
        {
            _writer.Emit(this, msg);
        }
    }
}
