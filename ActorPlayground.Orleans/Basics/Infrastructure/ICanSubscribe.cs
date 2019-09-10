using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface ICanSubscribe<TEvent> where TEvent : IHasStreamId
    {
        Task Subscribe(string subject, string provider);
    }
}
