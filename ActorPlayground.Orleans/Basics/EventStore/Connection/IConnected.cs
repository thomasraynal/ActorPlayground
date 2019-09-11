using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public interface IConnected<T>
    {
        T Value { get; }
        bool IsConnected { get; }
    }
}
