using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public class Observer<TEvent,TObserverGrain> : IAsyncObserver<TEvent> where TObserverGrain : IObserverGrain<TEvent>
    {
        private readonly TObserverGrain hostingGrain;

        internal Observer(TObserverGrain hostingGrain)
        {
            this.hostingGrain = hostingGrain;
        }

        public Task OnNextAsync(TEvent item, StreamSequenceToken token = null)
        {
           return hostingGrain.OnNext(item);
        }

        public Task OnCompletedAsync()
        {
            return hostingGrain.OnCompleted();
        }

        public Task OnErrorAsync(Exception ex)
        {
            return hostingGrain.OnError(ex);
        }
    }
}
