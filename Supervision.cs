using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public enum SupervisorDirective
    {
        Resume,
        Restart,
        Stop,
        Escalate
    }

    public interface ISupervisor
    {
        IImmutableSet<PID> Children { get; }
        void EscalateFailure(Exception reason, object message);
        void RestartChildren(Exception reason, params PID[] pids);
        void StopChildren(params PID[] pids);
        void ResumeChildren(params PID[] pids);
    }

    public static class Supervision
    {
        public static ISupervisorStrategy DefaultStrategy { get; } =
            new OneForOneStrategy((who, reason) => SupervisorDirective.Restart, 10, TimeSpan.FromSeconds(10));
        public static ISupervisorStrategy AlwaysRestartStrategy { get; } = new AlwaysRestartStrategy();
    }

    public interface ISupervisorStrategy
    {
        void HandleFailure(ISupervisor supervisor, PID child, Exception cause, object message);
    }

    public delegate SupervisorDirective Decider(PID pid, Exception reason);

    /// <summary>
    /// AllForOneStrategy returns a new SupervisorStrategy which applies the given fault Directive from the decider to the
    /// failing child and all its children.
    ///
    /// This strategy is appropriate when the children have a strong dependency, such that and any single one failing would
    /// place them all into a potentially invalid state.
    /// </summary>
    public class AllForOneStrategy : ISupervisorStrategy
    {
        private readonly Decider _decider;
        private readonly int _maxNrOfRetries;
        private readonly TimeSpan? _withinTimeSpan;

        public AllForOneStrategy(Decider decider, int maxNrOfRetries, TimeSpan? withinTimeSpan)
        {
            _decider = decider;
            _maxNrOfRetries = maxNrOfRetries;
            _withinTimeSpan = withinTimeSpan;
        }

        public void HandleFailure(ISupervisor supervisor, PID child, Exception reason, object message)
        {
            var directive = _decider(child, reason);
            switch (directive)
            {
                case SupervisorDirective.Resume:
                    supervisor.ResumeChildren(child);
                    break;
                case SupervisorDirective.Restart:
                        supervisor.RestartChildren(reason, supervisor.Children.ToArray());
                    break;
                case SupervisorDirective.Stop:
                    supervisor.StopChildren(supervisor.Children.ToArray());
                    break;
                case SupervisorDirective.Escalate:
                    supervisor.EscalateFailure(reason, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public class OneForOneStrategy : ISupervisorStrategy
    {
        private readonly Decider _decider;
        private readonly int _maxNrOfRetries;
        private readonly TimeSpan? _withinTimeSpan;

        public OneForOneStrategy(Decider decider, int maxNrOfRetries, TimeSpan? withinTimeSpan)
        {
            _decider = decider;
            _maxNrOfRetries = maxNrOfRetries;
            _withinTimeSpan = withinTimeSpan;
        }

        public void HandleFailure(ISupervisor supervisor, PID child, Exception reason, object message)
        {
            var directive = _decider(child, reason);
            switch (directive)
            {
                case SupervisorDirective.Resume:
                    supervisor.ResumeChildren(child);
                    break;
                case SupervisorDirective.Restart:
                    supervisor.RestartChildren(reason, child);
                    break;
                case SupervisorDirective.Stop:
                    supervisor.StopChildren(child);
                    break;
                case SupervisorDirective.Escalate:
                    supervisor.EscalateFailure(reason, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }

    public class AlwaysRestartStrategy : ISupervisorStrategy
    {
        public void HandleFailure(ISupervisor supervisor, PID child, Exception reason, object message)
        {
            //always restart
            supervisor.RestartChildren(reason, child);
        }
    }
}
