using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class ActorProcessConfiguration : IActorProcessConfiguration
    {
        public ActorProcessConfiguration(ActorId id, Func<IActor> actorFactory, IActorProcess parent, ActorType type)
        {
            Id = id;
            ActorFactory = actorFactory;
            Parent = parent;
            Type = type;
        }

        public ActorProcessConfiguration(ActorId id, Func<IActor> actorFactory, IActorProcess parent, ActorType type, Uri uri)
        {
            Id = id;
            ActorFactory = actorFactory;
            Parent = parent;
            Type = type;
            Uri = uri;
        }

        public ActorId Id { get; }
        public Func<IActor> ActorFactory { get; }
        public IActorProcess Parent { get; }
        public ActorType Type { get; }
        public Uri Uri { get; }
    }
}
