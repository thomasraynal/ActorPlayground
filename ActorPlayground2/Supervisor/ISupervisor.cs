using ActorPlayground.POC.Message;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ISupervisor
    {
        Task HandleFailure(Failure failure);
    }
}