using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public interface IDispatcher
    {
        int Throughput { get; }
        void Schedule(Func<Task> runner);
    }
}
