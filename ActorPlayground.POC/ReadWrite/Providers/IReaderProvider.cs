using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IReaderProvider
    {
        IReader Get(ActorId target);
    }
}
