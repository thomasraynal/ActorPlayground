using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface ISupervisor : IActor
    {
        ActorProcess Process { get; }
    }
}