using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ActorPlayground
{
    public abstract class Process
    {
        protected internal abstract void SendUserMessage(PID pid, object message);

        public virtual void Stop(PID pid)
        {
            SendSystemMessage(pid, ActorPlayground.Stop.Instance);
        }

        protected internal abstract void SendSystemMessage(PID pid, object message);
    }

    public class ActorProcess : Process
    {
        private long _isDead;

        public ActorProcess(IMailbox mailbox)
        {
            Mailbox = mailbox;
        }

        public IMailbox Mailbox { get; }

        internal bool IsDead
        {
            get => Interlocked.Read(ref _isDead) == 1;
            private set => Interlocked.Exchange(ref _isDead, value ? 1 : 0);
        }

        protected internal override void SendUserMessage(PID pid, object message)
        {
            Mailbox.PostUserMessage(message);
        }

        protected internal override void SendSystemMessage(PID pid, object message)
        {
            Mailbox.PostSystemMessage(message);
        }

        public override void Stop(PID pid)
        {
            base.Stop(pid);
            IsDead = true;
        }
    }
}
