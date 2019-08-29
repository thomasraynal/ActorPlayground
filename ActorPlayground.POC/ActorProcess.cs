using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ActorPlayground.POC
{
    public class ActorProcess
    {

        private readonly CancellationTokenSource _cancel;
        private readonly IActorRegistry _registry;


        //refacto shoud be string, handle all via cluster
        public List<ActorProcess> Children { get; }
        public ActorProcess Parent { get; private set; }
        public string Id { get; private set; }
        public Func<IActor> ActorFactory { get; private set; }
        public IActor Actor { get; private set; }
        public CancellationToken Token { get; }
        public Mailbox Mailbox { get; private set; }

        public ActorProcess(IActorRegistry registry)
        {
            _cancel = new CancellationTokenSource();
            _registry = registry;

            Children = new List<ActorProcess>();
            Token = _cancel.Token;
        }

        public void Initialize(string id, Func<IActor> actorFactory, Mailbox mailbox, ActorProcess parent)
        {
            Id = id;
            ActorFactory = actorFactory;
            Actor = actorFactory();
            Parent = parent;
            Mailbox = mailbox;
        }

        public ActorProcess SpawnChild(Func<IActor> actorFactory)
        {
            var child = _registry.Add(actorFactory, Parent);

            Children.Add(child);

            return child;

        }

        public void Post(object msg, ActorProcess sender)
        {
            Mailbox.Post(msg, sender);
        }
    }
}
