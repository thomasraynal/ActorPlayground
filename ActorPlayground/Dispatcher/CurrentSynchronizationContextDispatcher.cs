using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{

    public sealed class CurrentSynchronizationContextDispatcher : IDispatcher
    {
        private readonly TaskScheduler _scheduler;

        public CurrentSynchronizationContextDispatcher()
        {
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Throughput = 300;
        }

        public void Schedule(Func<Task> runner) => Task.Factory.StartNew(runner, CancellationToken.None, TaskCreationOptions.None, _scheduler);

        public int Throughput { get; set; }
    }
}
