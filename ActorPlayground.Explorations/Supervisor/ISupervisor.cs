using ActorPlayground.Explorations.Message;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface ISupervisor
    {
        Task HandleFailure(Failure failure);
    }
}