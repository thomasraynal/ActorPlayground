using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public interface IRootRemoteConfiguration
    {
        string Adress { get; }
        Func<IActor> ActorFactory { get; }

    }
}

