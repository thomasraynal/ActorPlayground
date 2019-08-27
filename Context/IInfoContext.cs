using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public interface IInfoContext
    {
        PID Parent { get; }
        PID Self { get; }
        PID Sender { get; }
        IActor Actor { get; }
    }
}
