using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    //todo: overload AsyncStreamHolderState
    public abstract class Consumer<TEvent> : Grain<AsyncStreamHolderState<TEvent>>, IAsyncObserver<TEvent>, ICanConnect, ICanSubscribe where TEvent : IEvent
    {

        public async Task Connect(string provider)
        {
       
            await Disconnect();

            State.Provider = provider;
   
            await base.WriteStateAsync();

        }

        public abstract Task OnNextAsync(TEvent @event, StreamSequenceToken token);

        public virtual Task OnErrorAsync(Exception exception)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public async Task Disconnect()
        {
            foreach (var subject in State.StreamHandles.Keys)
            {
                await State.StreamHandles[subject].UnsubscribeAsync();
            }

            State.StreamHandles.Clear();
        }

        public async override Task OnActivateAsync()
        {
            foreach (var subject in State.StreamHandles.Keys.ToList())
            {
                //bug? ResumeAsync does not work
                //State.StreamHandles[subject] = await State.StreamHandles[subject].ResumeAsync(this);

                await State.StreamHandles[subject].UnsubscribeAsync();

                var streamProvider = base.GetStreamProvider(State.Provider);
                var consumer = streamProvider.GetStream<TEvent>(Guid.Empty, subject);
                State.StreamHandles[subject] = await consumer.SubscribeAsync(this);

                await base.WriteStateAsync();

            }
        }

        public async Task Unsubscribe(string subject)
        {
            await State.StreamHandles[subject].UnsubscribeAsync();

            State.StreamHandles.Remove(subject);

            await base.WriteStateAsync();

        }

        public async Task Subscribe(string subject)
        {
            if (!State.StreamHandles.ContainsKey(subject))
            {
                var streamProvider = base.GetStreamProvider(State.Provider);
                var consumer = streamProvider.GetStream<TEvent>(Guid.Empty, subject);

                State.StreamHandles[subject] = await consumer.SubscribeAsync(this);

                await base.WriteStateAsync();
            }
        }
    }
}
