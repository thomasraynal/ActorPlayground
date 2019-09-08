using ActorPlayground.Explorations.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface ISupervisorStrategy
    {
        Task HandleFailure(IActorProcess self, Failure failure);
    }
}
