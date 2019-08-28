using System;
using System.Collections.Immutable;

namespace ActorPlayground
{
    internal class GuardianProcess : Process, ISupervisor
    {
        private readonly ISupervisorStrategy _supervisorStrategy;

        internal GuardianProcess(ISupervisorStrategy strategy)
        {
            _supervisorStrategy = strategy;

            var name = $"Guardian{ProcessRegistry.Instance.NextId()}";
            var (pid, ok) = ProcessRegistry.Instance.TryAdd(name, this);
            if (!ok)
            {
                throw new Exception($"{name} {pid}");
            }

            Pid = pid;
        }

        public PID Pid { get; }

        public IImmutableSet<PID> Children =>
            throw new MemberAccessException("Guardian does not hold its children PIDs.");

        public void EscalateFailure(Exception reason, object message)
        {
            throw new InvalidOperationException("Guardian cannot escalate failure.");
        }

        public void RestartChildren(Exception reason, params PID[] pids) =>
            pids?.SendSystemNessage(new Restart(reason));

        public void StopChildren(params PID[] pids) => pids?.Stop();

        public void ResumeChildren(params PID[] pids) => pids?.SendSystemNessage(ResumeMailbox.Instance);

        protected internal override void SendUserMessage(PID pid, object message)
        {
            throw new InvalidOperationException($"Guardian actor cannot receive any user messages.");
        }

        protected internal override void SendSystemMessage(PID pid, object message)
        {
            if (message is Failure msg)
            {
                _supervisorStrategy.HandleFailure(this, msg.Who, msg.Reason, msg.Message);
            }
        }
    }
}
