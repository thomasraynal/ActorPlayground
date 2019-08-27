using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public interface IMailboxQueue
    {
        bool HasMessages { get; }
        void Push(object message);
        object Pop();
    }
}
