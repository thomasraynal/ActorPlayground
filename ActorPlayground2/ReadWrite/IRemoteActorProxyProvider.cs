using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IRemoteActorProxyProvider
    {
        ICanPost Get(string id);
    }
}
