using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public sealed class ThreadPoolDispatcher : IDispatcher
    {
        public ThreadPoolDispatcher()
        {
            Throughput = 300;
        }

        public void Schedule(Func<Task> runner) => Task.Factory.StartNew(runner, TaskCreationOptions.None);

        public int Throughput { get; set; }
    }
}
