using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public sealed class SynchronousDispatcher : IDispatcher
    {
        public int Throughput => 300;

        public void Schedule(Func<Task> runner)
        {
            runner().Wait();
        }
    }
}
