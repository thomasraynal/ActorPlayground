using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface IObserverGrain<TEvent>
    {
        Task OnNext(TEvent @event);
        Task OnError(Exception error);
        Task OnCompleted();
    }
}
