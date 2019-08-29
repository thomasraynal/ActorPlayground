using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{

    internal readonly struct MessageContext : IContext
    {
  
        public MessageContext(ActorProcess actor, object message, ActorProcess sender)
        {
            Actor = actor;
            Message = message;
            Sender = sender;
        }

        public IEnumerable<string> Children => Actor.Children.Select(actor => actor.Id);

        public ActorProcess Actor { get; }

        public object Message { get; }

        public ActorProcess Sender { get; }

        public void Respond(object message)
        {
            Sender.Post(message, Actor);
        }
    }

    public class Mailbox
    {
        private readonly BlockingCollection<MessageContext> _messages;
        private readonly ActorProcess _process;
        private readonly Task _workProc;

        public Mailbox(ActorProcess process)
        {
            _messages = new BlockingCollection<MessageContext>();
            _process = process;
            _workProc = Task.Run(DoWork, process.Token);
        }

        public void Post(object msg, ActorProcess sender)
        {
            var context = new MessageContext(_process, msg, sender);

            _messages.Add(context);
        }

        private async Task DoWork()
        {
            foreach(var msg in _messages.GetConsumingEnumerable(_process.Token))
            {
                try
                {
                    await _process.Actor.Receive(msg);
                }
                catch(Exception ex)
                {

                }
            }
        }

        public void Respond(object message)
        {
            throw new NotImplementedException();
        }

    }
}
