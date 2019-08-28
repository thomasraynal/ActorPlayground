using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{

    internal class DefaultMailbox : IMailbox
    {
        private readonly ConcurrentQueue<object> _systemMessages;
        private readonly ConcurrentQueue<object> _userMailbox;

        private int _status = MailboxStatus.Idle;
        private long _systemMessageCount;
        private bool _suspended;

        internal int Status => _status;

        public IMessageInvoker Invoker { get; private set; }
        public IDispatcher Dispatcher { get; private set; }

        public DefaultMailbox()
        {
            _systemMessages = new ConcurrentQueue<object>();
            _userMailbox = new ConcurrentQueue<object>();
        }

        public void PostUserMessage(object msg)
        {
            _userMailbox.Enqueue(msg);
            Schedule();
        }

        public void PostSystemMessage(object msg)
        {
            _systemMessages.Enqueue(msg);
            Interlocked.Increment(ref _systemMessageCount);
            Schedule();
        }

        public void RegisterHandlers(IMessageInvoker invoker, IDispatcher dispatcher)
        {
            Invoker = invoker;
            Dispatcher = dispatcher;
        }

        public void Start()
        {
        }

        private Task RunAsync()
        {
            var done = ProcessMessages();

            if (!done)
                // mailbox is halted, awaiting completion of a message task, upon which mailbox will be rescheduled
                return Task.FromResult(0);

            Interlocked.Exchange(ref _status, MailboxStatus.Idle);

            if (_systemMessages.IsEmpty || !_suspended && _userMailbox.IsEmpty)
            {
                Schedule();
            }

            return Task.FromResult(0);
        }

        private bool ProcessMessages()
        {
            object msg = null;
            try
            {
                for (var i = 0; i < Dispatcher.Throughput; i++)
                {
                    if (Interlocked.Read(ref _systemMessageCount) > 0 && _systemMessages.TryDequeue(out msg))
                    {
                        Interlocked.Decrement(ref _systemMessageCount);
            
    
                        var t = Invoker.InvokeSystemMessageAsync(msg);
                        if (t.IsFaulted)
                        {
                            Invoker.EscalateFailure(t.Exception,  msg);
                            continue;
                        }
                        if (!t.IsCompleted)
                        {
                            // if task didn't complete immediately, halt processing and reschedule a new run when task completes
                            t.ContinueWith(RescheduleOnTaskComplete, msg);
                            return false;
                        }
                        continue;
                    }
                    if (_suspended)
                    {
                        break;
                    }
                    if (_userMailbox.TryDequeue(out msg))
                    {
                        var t = Invoker.InvokeUserMessageAsync(msg);
                        if (t.IsFaulted)
                        {
                            Invoker.EscalateFailure(t.Exception, msg);
                            continue;
                        }
                        if (!t.IsCompleted)
                        {
                            // if task didn't complete immediately, halt processing and reschedule a new run when task completes
                            t.ContinueWith(RescheduleOnTaskComplete, msg);
                            return false;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Invoker.EscalateFailure(e, msg);
            }
            return true;
        }

        private void RescheduleOnTaskComplete(Task task, object message)
        {
            if (task.IsFaulted)
            {
                Invoker.EscalateFailure(task.Exception, message);
            }

            Dispatcher.Schedule(RunAsync);
        }

        protected void Schedule()
        {
            if (Interlocked.CompareExchange(ref _status, MailboxStatus.Busy, MailboxStatus.Idle) == MailboxStatus.Idle)
            {
                Dispatcher.Schedule(RunAsync);
            }
        }
    }
}
