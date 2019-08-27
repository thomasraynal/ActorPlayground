﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{
    internal static class MailboxStatus
    {
        public const int Idle = 0;
        public const int Busy = 1;
    }

    public interface IMailbox
    {
        void PostUserMessage(object msg);
        void PostSystemMessage(object msg);
        void RegisterHandlers(IMessageInvoker invoker, IDispatcher dispatcher);
        void Start();
    }

    public static class BoundedMailbox
    {
        public static IMailbox Create(int size, params IMailboxStatistics[] stats) => new DefaultMailbox(new UnboundedMailboxQueue(), new UnboundedMailboxQueue(), stats);
    }

    public static class UnboundedMailbox
    {
        public static IMailbox Create(params IMailboxStatistics[] stats) => new DefaultMailbox(new UnboundedMailboxQueue(), new UnboundedMailboxQueue(), stats);
    }

    internal class DefaultMailbox : IMailbox
    {
        private readonly IMailboxStatistics[] _stats;
        private readonly IMailboxQueue _systemMessages;
        private readonly IMailboxQueue _userMailbox;
        private IDispatcher _dispatcher;
        private IMessageInvoker _invoker;

        private int _status = MailboxStatus.Idle;
        private long _systemMessageCount;
        private bool _suspended;

        internal int Status => _status;

        public DefaultMailbox(IMailboxQueue systemMessages, IMailboxQueue userMailbox, params IMailboxStatistics[] stats)
        {
            _systemMessages = systemMessages;
            _userMailbox = userMailbox;
            _stats = stats ?? new IMailboxStatistics[0];
        }

        public void PostUserMessage(object msg)
        {
            _userMailbox.Push(msg);
            foreach (var t in _stats)
            {
                t.MessagePosted(msg);
            }
            Schedule();
        }

        public void PostSystemMessage(object msg)
        {
            _systemMessages.Push(msg);
            Interlocked.Increment(ref _systemMessageCount);
            foreach (var t in _stats)
            {
                t.MessagePosted(msg);
            }
            Schedule();
        }

        public void RegisterHandlers(IMessageInvoker invoker, IDispatcher dispatcher)
        {
            _invoker = invoker;
            _dispatcher = dispatcher;
        }

        public void Start()
        {
            foreach (var t in _stats)
            {
                t.MailboxStarted();
            }
        }

        private Task RunAsync()
        {
            var done = ProcessMessages();

            if (!done)
                // mailbox is halted, awaiting completion of a message task, upon which mailbox will be rescheduled
                return Task.FromResult(0);

            Interlocked.Exchange(ref _status, MailboxStatus.Idle);

            if (_systemMessages.HasMessages || !_suspended && _userMailbox.HasMessages)
            {
                Schedule();
            }
            else
            {
                foreach (var t in _stats)
                {
                    t.MailboxEmpty();
                }
            }
            return Task.FromResult(0);
        }

        private bool ProcessMessages()
        {
            object msg = null;
            try
            {
                for (var i = 0; i < _dispatcher.Throughput; i++)
                {
                    if (Interlocked.Read(ref _systemMessageCount) > 0 && (msg = _systemMessages.Pop()) != null)
                    {
                        Interlocked.Decrement(ref _systemMessageCount);
            
    
                        var t = _invoker.InvokeSystemMessageAsync(msg);
                        if (t.IsFaulted)
                        {
                            _invoker.EscalateFailure(t.Exception, msg);
                            continue;
                        }
                        if (!t.IsCompleted)
                        {
                            // if task didn't complete immediately, halt processing and reschedule a new run when task completes
                            t.ContinueWith(RescheduleOnTaskComplete, msg);
                            return false;
                        }
                        foreach (var t1 in _stats)
                        {
                            t1.MessageReceived(msg);
                        }
                        continue;
                    }
                    if (_suspended)
                    {
                        break;
                    }
                    if ((msg = _userMailbox.Pop()) != null)
                    {
                        var t = _invoker.InvokeUserMessageAsync(msg);
                        if (t.IsFaulted)
                        {
                            _invoker.EscalateFailure(t.Exception, msg);
                            continue;
                        }
                        if (!t.IsCompleted)
                        {
                            // if task didn't complete immediately, halt processing and reschedule a new run when task completes
                            t.ContinueWith(RescheduleOnTaskComplete, msg);
                            return false;
                        }
                        foreach (var t1 in _stats)
                        {
                            t1.MessageReceived(msg);
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
                _invoker.EscalateFailure(e, msg);
            }
            return true;
        }

        private void RescheduleOnTaskComplete(Task task, object message)
        {
            if (task.IsFaulted)
            {
                _invoker.EscalateFailure(task.Exception, message);
            }
            else
            {
                foreach (var t in _stats)
                {
                    t.MessageReceived(message);
                }
            }
            _dispatcher.Schedule(RunAsync);
        }


        protected void Schedule()
        {
            if (Interlocked.CompareExchange(ref _status, MailboxStatus.Busy, MailboxStatus.Idle) == MailboxStatus.Idle)
            {
                _dispatcher.Schedule(RunAsync);
            }
        }
    }

    /// <summary>
    /// Extension point for getting notifications about mailbox events
    /// </summary>
    public interface IMailboxStatistics
    {
        /// <summary>
        /// This method is invoked when the mailbox is started
        /// </summary>
        void MailboxStarted();
        /// <summary>
        /// This method is invoked when a message is posted to the mailbox.
        /// </summary>
        void MessagePosted(object message);
        /// <summary>
        /// This method is invoked when a message has been received by the invoker associated with the mailbox.
        /// </summary>
        void MessageReceived(object message);
        /// <summary>
        /// This method is invoked when all messages in the mailbox have been received.
        /// </summary>
        void MailboxEmpty();
    }
}
