using ActorPlayground.POC.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{

    public class Mailbox
    {
        private readonly BlockingCollection<MessageContext> _messages;
        private readonly ActorProcess _process;
        private readonly Task _workProc;
        private readonly ISupervisor _supervisor;

        public Mailbox(ActorProcess process, ISupervisor supervisor)
        {
            _messages = new BlockingCollection<MessageContext>();
            _process = process;
            _supervisor = supervisor;
            _workProc = Task.Run(DoWork, process.Token);
        }

        public void Post(object msg, ActorProcess sender)
        {
            var context = new MessageContext(_process, msg, sender);

            _messages.Add(context);
        }

        private async Task DoWork()
        {
            foreach (var msg in _messages.GetConsumingEnumerable(_process.Token))
            {
                try
                {
                    await _process.Actor.Receive(msg);
                }
                catch (Exception ex)
                {
                    var message = new Failure(msg.Actor.Id, ex);
                    var failure = new MessageContext(msg.Actor, message, _process);

                    await _supervisor.Receive(failure);

                }
            }
        }

    }
}
