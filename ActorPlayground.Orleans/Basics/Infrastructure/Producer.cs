using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public abstract class Producer<TEvent> : Grain, ICanConnect where TEvent : IHasStreamId
    {

        private Dictionary<string, IAsyncStream<TEvent>> _streams;
        private string _provider;

        public async override Task OnActivateAsync()
        {
            _streams = new Dictionary<string, IAsyncStream<TEvent>>();

            await base.OnActivateAsync();
        }

        public Task Connect(string provider)
        {
            _provider = provider;
            return Task.CompletedTask;
        }

        public async Task Next(TEvent @event)
        {
            if (!_streams.ContainsKey(@event.StreamId))
            {
                var streamProvider = base.GetStreamProvider(_provider);
                var producer = streamProvider.GetStream<TEvent>(Guid.Empty, @event.StreamId);
                _streams[@event.StreamId] = producer;
            }

            await _streams[@event.StreamId].OnNextAsync(@event);

        }

    }
}
