using System.Collections.Concurrent;


namespace ActorPlayground
{
    public static class Guardians
    {
        private static readonly ConcurrentDictionary<ISupervisorStrategy, GuardianProcess> GuardianStrategies = new ConcurrentDictionary<ISupervisorStrategy, GuardianProcess>();

        internal static PID GetGuardianPID(ISupervisorStrategy strategy)
        {
            GuardianProcess ValueFactory(ISupervisorStrategy s) => new GuardianProcess(s);

            var guardian = GuardianStrategies.GetOrAdd(strategy, ValueFactory);
            return guardian.Pid;
        }
    }
}
