using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public abstract class Consumer<TEvent> : Grain, IAsyncObserver<TEvent>
    {
        private IAsyncStream<TEvent> _consumer;
        private StreamSubscriptionHandle<TEvent> _handle;

        public async Task Connect(string streamNamespace, string providerToUse)
        {
            var streamProvider = base.GetStreamProvider(providerToUse);
            _consumer = streamProvider.GetStream<TEvent>(Guid.Empty, streamNamespace);
            _handle = await _consumer.SubscribeAsync(this);

        }

        public abstract Task OnCompletedAsync();

        public abstract Task OnErrorAsync(Exception ex);

        public abstract Task OnNextAsync(TEvent item, StreamSequenceToken token = null);

 
    }
}
