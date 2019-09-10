using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public abstract class Producer<TEvent> : Grain, IObserverGrain<TEvent>, ICanConnect
    {
        private IAsyncStream<TEvent> _producer;

        public Task Connect(string streamNamespace, string providerToUse)
        {
            var streamProvider = base.GetStreamProvider(providerToUse);
            _producer = streamProvider.GetStream<TEvent>(Guid.Empty, streamNamespace);

            return Task.CompletedTask;
        }

        public async Task OnCompleted()
        {
            await _producer.OnCompletedAsync();
        }

        public async Task OnError(Exception error)
        {
            await _producer.OnErrorAsync(error);
        }

        public async Task OnNext(TEvent @event)
        {
           await _producer.OnNextAsync(@event);
        }
    }
}
