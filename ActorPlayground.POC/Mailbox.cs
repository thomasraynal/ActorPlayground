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
    public class Mailbox : IMailbox
    {
        private BlockingCollection<MessageContext> _messages;
        private readonly IActorProcess _process;
        private CancellationTokenSource _cancel;
        private Task _workProc;

        public Mailbox(IActorProcess process)
        {
            _process = process;
        }

        public void Post(IMessage msg, IActorProcess sender)
        {
            var context = new MessageContext(_process, msg, sender);

            _messages.Add(context);
        }

        public void Start()
        {
            _messages = new BlockingCollection<MessageContext>();
            _cancel = new CancellationTokenSource();
            _workProc = Task.Run(DoWork, _cancel.Token);

        }

        public void Stop()
        {
            _cancel.Cancel();
            while (!_messages.IsCompleted)  Thread.Sleep(10);
        }

        private async Task DoWork()
        {
            foreach (var msg in _messages.GetConsumingEnumerable(_cancel.Token))
            {
                try
                {
                    if (msg.Message.IsSystemMessage)
                    {
                        _process.HandleSystemMessage(msg.Message);
                        continue;
                    }

                    await _process.Actor.Receive(msg);
                }
                catch (Exception ex)
                {
                    var failure = new Failure(msg.Actor.Id, ex);

                    await _process.HandleFailure(failure);

                }
            }
        }

    }
}
