using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IRemoteWriterProvider
    {
        IWriter Get(ActorId target, ICanPost sender);
    }
}
