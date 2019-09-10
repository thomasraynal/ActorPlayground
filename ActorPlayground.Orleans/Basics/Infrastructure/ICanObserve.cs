using System;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface ICanObserve<TEvent> : ICanSubscribe<TEvent> where TEvent : IHasStreamId
    {
        Task OnError(Exception exception);
        Task OnCompleted();
    }
}