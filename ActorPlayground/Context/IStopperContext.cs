using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public interface IStopperContext
    {
        void Stop(PID pid);

        Task StopAsync(PID pid);

        void Poison(PID pid);

        Task PoisonAsync(PID pid);
    }
}
