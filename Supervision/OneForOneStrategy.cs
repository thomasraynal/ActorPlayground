using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class OneForOneStrategy : ISupervisorStrategy
    {
        private readonly Decider _decider;

        public OneForOneStrategy(Decider decider)
        {
            _decider = decider;
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

}
