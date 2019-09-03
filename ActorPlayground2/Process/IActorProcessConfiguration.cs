using System;

namespace ActorPlayground.POC
{
    public interface IActorProcessConfiguration
    {
        Func<IActor> ActorFactory { get; }
        ActorId Id { get; }
        ICanPost Parent { get; }
        ActorType Type { get; }
        Uri Uri { get;  }
    }
}