using ActorPlayground.POC.Message;
using System.Collections.Generic;
using System.Threading;

namespace ActorPlayground.POC
{
    public class ActorProcess
    {

        private readonly CancellationTokenSource _cancel;
        private readonly IActorRegistry _registry;
        private readonly ISupervisor _supervisor;

        //refacto shoud be string, handle all via cluster
        public List<ActorProcess> Children { get; }
        public ActorProcess Parent { get; private set; }
        public string Id { get; private set; }
        public IActor Actor { get; private set; }
        public CancellationToken Token { get; }
        public Mailbox Mailbox { get; }

        public ActorProcess(ISupervisor supervisor, IActorRegistry registry)
        {
            _cancel = new CancellationTokenSource();
            _registry = registry;
            _supervisor = supervisor;

            Children = new List<ActorProcess>();
            Token = _cancel.Token;
            Mailbox = new Mailbox(this, supervisor);
        }

        public void Initialize(string id, IActor actor, ActorProcess parent)
        {
            Id = id;
            Actor = actor;
            Parent = parent;
        }

        public ActorProcess SpawnChild(IActor actor)
        {
            var child = _registry.Add(actor, Parent);

            Children.Add(child);

            return child;

        }

        public void Post(object msg, ActorProcess sender)
        {
            Mailbox.Post(msg, sender);
        }

        public void Start()
        {
            Post(new Start(), this);
        }

        public void Stop()
        {
            Post(new Stop(), this);

            foreach (var child in Children)
            {
                child.Stop();
            }

            _cancel.Cancel();
        }
    }
}
