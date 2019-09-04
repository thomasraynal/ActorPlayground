using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IMailbox : IProcess
    {
        void Post(IEvent msg, ICanPost sender);
    }
}