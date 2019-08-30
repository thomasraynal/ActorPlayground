using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IMailbox
    {
        void Post(IMessage msg, IActorProcess sender);
        void Start();
        void Stop();
    }
}