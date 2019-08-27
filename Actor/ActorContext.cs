﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{
    internal enum ContextState : byte
    {
        Alive,
        Restarting,
        Stopping,
        Stopped,
    }

    //Angels cry over this code, but it serves a purpose, lazily init of less frequently used features
    public class ActorContextExtras
    {
        public ImmutableHashSet<PID> Children { get; private set; } = ImmutableHashSet<PID>.Empty;
        public Timer ReceiveTimeoutTimer { get; private set; }
        public Stack<object> Stash { get; } = new Stack<object>();
        public ImmutableHashSet<PID> Watchers { get; private set; } = ImmutableHashSet<PID>.Empty;
        public IContext Context { get; }

        public ActorContextExtras(IContext context)
        {
            Context = context;
        }

        public void InitReceiveTimeoutTimer(Timer timer) => ReceiveTimeoutTimer = timer;

        public void ResetReceiveTimeoutTimer(TimeSpan timeout) => ReceiveTimeoutTimer?.Change(timeout, timeout);

        public void StopReceiveTimeoutTimer() => ReceiveTimeoutTimer?.Change(-1, -1);

        public void KillreceiveTimeoutTimer()
        {
            ReceiveTimeoutTimer.Dispose();
            ReceiveTimeoutTimer = null;
        }

        public void AddChild(PID pid) => Children = Children.Add(pid);

        public void RemoveChild(PID msgWho) => Children = Children.Remove(msgWho);

        public void Watch(PID watcher)
        {
            Watchers = Watchers.Add(watcher);
        }

        public void Unwatch(PID watcher)
        {
            Watchers = Watchers.Remove(watcher);
        }

        internal void RemoveChild(object who)
        {
            throw new NotImplementedException();
        }
    }

    public class ActorContext : IMessageInvoker, IContext, ISupervisor
    {
        public static readonly ImmutableHashSet<PID> EmptyChildren = ImmutableHashSet<PID>.Empty;
        private readonly Props _props;

        private ActorContextExtras _extras;
        private object _messageOrEnvelope;
        private ContextState _state;

        private ActorContextExtras EnsureExtras()
        {
            if (_extras == null)
            {
                var context = _props?.ContextDecoratorChain(this) ?? this;
                _extras = new ActorContextExtras(context);
            }

            return _extras;
        }

        public ActorContext(Props props, PID parent)
        {
            _props = props;

            //Parents are implicitly watching the child
            //The parent is not part of the Watchers set
            Parent = parent;

            IncarnateActor();
        }

        public IImmutableSet<PID> Children => _extras?.Children ?? EmptyChildren;
        IReadOnlyCollection<PID> IContext.Children => Children;

        public IActor Actor { get; private set; }
        public PID Parent { get; }
        public PID Self { get; set; }

        public object Message => MessageEnvelope.UnwrapMessage(_messageOrEnvelope);

        public PID Sender => MessageEnvelope.UnwrapSender(_messageOrEnvelope);

        public MessageHeader Headers => MessageEnvelope.UnwrapHeader(_messageOrEnvelope);

        public TimeSpan ReceiveTimeout { get; private set; }

        public void Stash() => EnsureExtras().Stash.Push(Message);

        public void Respond(object message) => Send(Sender, message);

        public PID Spawn(Props props)
        {
            var id = ProcessRegistry.Instance.NextId();
            return SpawnNamed(props, id);
        }

        public PID SpawnPrefix(Props props, string prefix)
        {
            var name = prefix + ProcessRegistry.Instance.NextId();
            return SpawnNamed(props, name);
        }

        public PID SpawnNamed(Props props, string name)
        {
            if (props.GuardianStrategy != null)
            {
                throw new ArgumentException("Props used to spawn child cannot have GuardianStrategy.");
            }

            var pid = props.Spawn($"{Self.Id}/{name}", Self);
            EnsureExtras().AddChild(pid);


            return pid;
        }

        public void Watch(PID pid) => pid.SendSystemMessage(new Watch(Self));

        public void Unwatch(PID pid) => pid.SendSystemMessage(new Unwatch(Self));

        public void SetReceiveTimeout(TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration must be greater than zero");
            }

            if (duration == ReceiveTimeout)
            {
                return;
            }

            ReceiveTimeout = duration;

            EnsureExtras();
            _extras.StopReceiveTimeoutTimer();
            if (_extras.ReceiveTimeoutTimer == null)
            {
                _extras.InitReceiveTimeoutTimer(new Timer(ReceiveTimeoutCallback, null, ReceiveTimeout,
                    ReceiveTimeout));
            }
            else
            {
                _extras.ResetReceiveTimeoutTimer(ReceiveTimeout);
            }
        }

        public void CancelReceiveTimeout()
        {
            if (_extras?.ReceiveTimeoutTimer == null)
            {
                return;
            }
            _extras.StopReceiveTimeoutTimer();
            _extras.KillreceiveTimeoutTimer();

            ReceiveTimeout = TimeSpan.Zero;
        }

        public void Send(PID target, object message) => SendUserMessage(target, message);

        public void Forward(PID target)
        {
            if (_messageOrEnvelope is SystemMessage)
            {
                //SystemMessage cannot be forwarded
                return;
            }
            SendUserMessage(target, _messageOrEnvelope);
        }

        public void Request(PID target, object message)
        {
            var messageEnvelope = new MessageEnvelope(message, Self, null);
            SendUserMessage(target, messageEnvelope);
        }

        public void Request(PID target, object message, PID sender)
        {
            var messageEnvelope = new MessageEnvelope(message, sender, null);
            SendUserMessage(target, messageEnvelope);
        }

        public Task<T> RequestAsync<T>(PID target, object message, TimeSpan timeout)
            => RequestAsync(target, message, new FutureProcess<T>(timeout));

        public Task<T> RequestAsync<T>(PID target, object message, CancellationToken cancellationToken)
            => RequestAsync(target, message, new FutureProcess<T>(cancellationToken));

        public Task<T> RequestAsync<T>(PID target, object message)
            => RequestAsync(target, message, new FutureProcess<T>());

        public void ReenterAfter<T>(Task<T> target, Func<Task<T>, Task> action)
        {
            var msg = _messageOrEnvelope;
            var cont = new Continuation(() => action(target), msg);

            target.ContinueWith(t => { Self.SendSystemMessage(cont); });
        }

        public void ReenterAfter(Task target, Action action)
        {
            var msg = _messageOrEnvelope;
            var cont = new Continuation(() =>
            {
                action();
                return Task.CompletedTask;
            }, msg);

            target.ContinueWith(t => { Self.SendSystemMessage(cont); });
        }

        public void EscalateFailure(Exception reason, object message)
        {
            var failure = new Failure(Self, reason, message);
            Self.SendSystemMessage(SuspendMailbox.Instance);
            if (Parent == null)
            {
                HandleRootFailure(failure);
            }
            else
            {
                Parent.SendSystemMessage(failure);
            }
        }

        public void RestartChildren(Exception reason, params PID[] pids) => pids.SendSystemNessage(new Restart(reason));

        public void StopChildren(params PID[] pids) => pids.SendSystemNessage(ActorPlayground.Stop.Instance);

        public void ResumeChildren(params PID[] pids) => pids.SendSystemNessage(ResumeMailbox.Instance);

        public Task InvokeSystemMessageAsync(object msg)
        {
            try
            {
                switch (msg)
                {
                    case Started s:
                        return InvokeUserMessageAsync(s);
                    case Stop _:
                        return InitiateStopAsync();
                    case Terminated t:
                        return HandleTerminatedAsync(t);
                    case Watch w:
                        HandleWatch(w);
                        return Task.CompletedTask;
                    case Unwatch uw:
                        HandleUnwatch(uw);
                        return Task.CompletedTask;
                    case Failure f:
                        HandleFailure(f);
                        return Task.CompletedTask;
                    case Restart _:
                        return HandleRestartAsync();
                    case SuspendMailbox _:
                        return Task.CompletedTask;
                    case ResumeMailbox _:
                        return Task.CompletedTask;
                    case Continuation cont:
                        _messageOrEnvelope = cont.Message;
                        return cont.Action();
                    default:
                        return Task.CompletedTask;
                }
            }
            catch (Exception x)
            {
                throw;
            }
        }

        public Task InvokeUserMessageAsync(object msg)
        {
            if (_state == ContextState.Stopped)
            {
                return Task.CompletedTask;
            }

            var influenceTimeout = true;
            if (ReceiveTimeout > TimeSpan.Zero)
            {
                var notInfluenceTimeout = msg is INotInfluenceReceiveTimeout;
                influenceTimeout = !notInfluenceTimeout;
                if (influenceTimeout)
                {
                    _extras.StopReceiveTimeoutTimer();
                }
            }

            var res = ProcessMessageAsync(msg);

            if (ReceiveTimeout != TimeSpan.Zero && influenceTimeout)
            {
                //special handle non completed tasks that need to reset ReceiveTimout
                if (!res.IsCompleted)
                {
                    return res.ContinueWith(_ => _extras.ResetReceiveTimeoutTimer(ReceiveTimeout));
                }

                _extras.ResetReceiveTimeoutTimer(ReceiveTimeout);
            }
            return res;
        }

        public Task Receive(MessageEnvelope envelope)
        {
            _messageOrEnvelope = envelope;
            return DefaultReceive();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task DefaultReceive()
        {
            if (Message is PoisonPill)
            {
                Stop(Self);
                return Task.CompletedTask;
            }


            //are we using decorators, if so, ensure it has been created
            if (_props.ContextDecoratorChain != null)
            {
                return Actor.ReceiveAsync(EnsureExtras().Context);
            }

            return Actor.ReceiveAsync(this);
        }

        private Task ProcessMessageAsync(object msg)
        {
            //slow path, there is middleware, message must be wrapped in an envelop
            if (_props.ReceiverMiddlewareChain != null)
            {
                return _props.ReceiverMiddlewareChain(EnsureExtras().Context, MessageEnvelope.Wrap(msg));
            }
            if (_props.ContextDecoratorChain != null)
            {
                return EnsureExtras().Context.Receive(MessageEnvelope.Wrap(msg));
            }
            //fast path, 0 alloc invocation of actor receive
            _messageOrEnvelope = msg;
            return DefaultReceive();
        }

        private Task<T> RequestAsync<T>(PID target, object message, FutureProcess<T> future)
        {
            var messageEnvelope = new MessageEnvelope(message, future.Pid, null);
            SendUserMessage(target, messageEnvelope);
            return future.Task;
        }

        private void SendUserMessage(PID target, object message)
        {
            if (_props.SenderMiddlewareChain != null)
            {
                //slow path
                _props.SenderMiddlewareChain(EnsureExtras().Context, target, MessageEnvelope.Wrap(message));
            }
            else
            {
                //fast path, 0 alloc
                target.SendUserMessage(message);
            }
        }

        private void IncarnateActor()
        {
            _state = ContextState.Alive;
            Actor = _props.Producer();
        }

        private async Task HandleRestartAsync()
        {
            _state = ContextState.Restarting;
            CancelReceiveTimeout();
            await InvokeUserMessageAsync(Restarting.Instance);
            await StopAllChildren();
        }

        private void HandleUnwatch(Unwatch uw) => _extras?.Unwatch(uw.Watcher);

        private void HandleWatch(Watch w)
        {
            if (_state >= ContextState.Stopping)
            {
                w.Watcher.SendSystemMessage(Terminated.From(Self));
            }
            else
            {
                EnsureExtras().Watch(w.Watcher);
            }
        }

        private void HandleFailure(Failure msg)
        {
            switch (Actor)
            {
                case ISupervisorStrategy supervisor:
                    supervisor.HandleFailure(this, msg.Who, msg.Reason, msg.Message);
                    break;
                default:
                    _props.SupervisorStrategy.HandleFailure(this, msg.Who, msg.Reason, msg.Message);
                    break;
            }
        }

        private async Task HandleTerminatedAsync(Terminated msg)
        {
            _extras?.RemoveChild(msg.Who);
            await InvokeUserMessageAsync(msg);
            if (_state == ContextState.Stopping || _state == ContextState.Restarting)
            {
                await TryRestartOrStopAsync();
            }
        }

        private void HandleRootFailure(Failure failure)
        {
            Supervision.DefaultStrategy.HandleFailure(this, failure.Who, failure.Reason, failure.Message);
        }

        //Initiate stopping, not final
        private async Task InitiateStopAsync()
        {
            if (_state >= ContextState.Stopping)
            {
                //already stopping or stopped
                return;
            }

            _state = ContextState.Stopping;
            CancelReceiveTimeout();
            //this is intentional
            await InvokeUserMessageAsync(Stopping.Instance);
            await StopAllChildren();
        }

        private async Task StopAllChildren()
        {
            _extras?.Children?.Stop();
            await TryRestartOrStopAsync();
        }

        //intermediate stopping stage, waiting for children to stop
        private Task TryRestartOrStopAsync()
        {
            if (_extras?.Children?.Count > 0)
            {
                return Task.CompletedTask;
            }

            CancelReceiveTimeout();

            switch (_state)
            {
                case ContextState.Restarting:
                    return RestartAsync();
                case ContextState.Stopping:
                    return FinalizeStopAsync();
                default: return Task.CompletedTask;
            }
        }

        //Last and final termination step
        private async Task FinalizeStopAsync()
        {
            ProcessRegistry.Instance.Remove(Self);
            //This is intentional
            await InvokeUserMessageAsync(Stopped.Instance);

            DisposeActorIfDisposable();

            //Notify watchers
            _extras?.Watchers.SendSystemNessage(Terminated.From(Self));

            //Notify parent
            Parent?.SendSystemMessage(Terminated.From(Self));

            _state = ContextState.Stopped;
        }

        private async Task RestartAsync()
        {
            DisposeActorIfDisposable();
            IncarnateActor();
            Self.SendSystemMessage(ResumeMailbox.Instance);

            await InvokeUserMessageAsync(Started.Instance);
            if (_extras?.Stash != null)
            {
                while (_extras.Stash.Any())
                {
                    var msg = _extras.Stash.Pop();
                    await InvokeUserMessageAsync(msg);
                }
            }
        }

        private void DisposeActorIfDisposable()
        {
            if (Actor is IDisposable disposableActor)
            {
                disposableActor.Dispose();
            }
        }

        private void ReceiveTimeoutCallback(object state)
        {
            if (_extras?.ReceiveTimeoutTimer == null)
            {
                return;
            }
            CancelReceiveTimeout();
            Send(Self, ActorPlayground.ReceiveTimeout.Instance);
        }

        public void Stop(PID pid)
        {
            var reff = ProcessRegistry.Instance.Get(pid);
            reff.Stop(pid);
        }

        public Task StopAsync(PID pid)
        {
            var future = new FutureProcess<object>();

            pid.SendSystemMessage(new Watch(future.Pid));
            Stop(pid);

            return future.Task;
        }

        public void Poison(PID pid) => pid.SendUserMessage(new PoisonPill());

        public Task PoisonAsync(PID pid)
        {
            var future = new FutureProcess<object>();

            pid.SendSystemMessage(new Watch(future.Pid));
            Poison(pid);

            return future.Task;
        }
    }
}
