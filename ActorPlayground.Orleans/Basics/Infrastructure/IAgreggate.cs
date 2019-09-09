using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface IAgreggate
    {
        Task<IEnumerable<IEvent>> GetAppliedEvents();
    }
}
