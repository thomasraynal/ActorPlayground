using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public abstract class Consumer<TEvent> : Grain, ICanSubscribe<TEvent> where TEvent : IHasStreamId
    {
        private IAsyncStream<TEvent> _consumer;

        public abstract Task OnNext(TEvent @event);

        public Task OnError(Exception exception)
        {
            return Task.CompletedTask;
        }

        public Task OnCompleted()
        {
            return Task.CompletedTask;
        }

        public async override Task OnActivateAsync()
        {
            if(null != _consumer)
            {
                foreach(var handle in await _consumer.GetAllSubscriptionHandles())
                {
                   await handle.ResumeAsync((data, token) =>
                    {
                        OnNext(data);
                        return Task.CompletedTask;

                    }, OnError, OnCompleted);
                }
            }

            await base.OnActivateAsync();
        }

        public async Task Subscribe(string subject, string provider)
        {
            var streamProvider = GetStreamProvider(provider);

            _consumer = streamProvider.GetStream<TEvent>(Guid.Empty, subject);

            await _consumer.SubscribeAsync((data, token) =>
             {
                 OnNext(data);
                 return Task.CompletedTask;

             }, OnError, OnCompleted);

        }
    }
}
