using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ActorPlayground.POC
{
    public class ActorProcess
    {

        private readonly CancellationTokenSource _cancel;

        public ActorProcess(string id, IActor actor)
        {
            _cancel = new CancellationTokenSource();

            Id = id;
            Actor = actor;
            Token = _cancel.Token;
            Mailbox = new Mailbox(this);
        }

        public List<ActorProcess> Children { get; }
        public string Id { get; }
        public IActor Actor { get; }
        public CancellationToken Token { get; }
        public Mailbox Mailbox { get; }

        public void Post(object msg, ActorProcess sender)
        {
            Mailbox.Post(msg, sender);
        }

        public void Start()
        {

        }

        public void Stop()
        {
            _cancel.Cancel();
        }
    }
}
