using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface IMailbox : IProcess
    {
        void Post(IEvent msg, ICanPost sender);
    }
}