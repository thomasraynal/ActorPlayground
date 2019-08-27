using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ActorPlayground
{
    public class ProcessRegistry
    {
        private const string NoHost = "nonhost";
        private readonly IList<Func<PID, Process>> _hostResolvers = new List<Func<PID, Process>>();
        private readonly ConcurrentDictionary<string, Process> _localActorRefs = new ConcurrentDictionary<string, Process>();
        private int _sequenceId;
        public static ProcessRegistry Instance { get; } = new ProcessRegistry();
        public string Address { get; set; } = NoHost;

        public void RegisterHostResolver(Func<PID, Process> resolver)
        {
            _hostResolvers.Add(resolver);
        }

        public Process Get(PID pid)
        {
            if (pid.Address != NoHost && pid.Address != Address)
            {
                foreach (var resolver in _hostResolvers)
                {
                    var reff = resolver(pid);
                    if (reff == null)
                    {
                        continue;
                    }
                    return reff;
                }
                throw new NotSupportedException("Unknown host");
            }

            if (_localActorRefs.TryGetValue(pid.Id, out var process))
            {
                return process;
            }

            throw new Exception("not found");

        }

        public Process GetLocal(string id)
        {
            if (_localActorRefs.TryGetValue(id, out var process))
            {
                return process;
            }

            throw new Exception("not found");
        }

        public (PID pid, bool ok) TryAdd(string id, Process process)
        {
            var pid = new PID(Address, id, process);

            var ok = _localActorRefs.TryAdd(pid.Id, process);
            return ok ? (pid, true) : (new PID(Address, id), false);
        }

        public void Remove(PID pid)
        {
            _localActorRefs.TryRemove(pid.Id, out var process);
        }

        public string NextId()
        {
            var counter = Interlocked.Increment(ref _sequenceId);
            return "$" + counter;
        }
    }
}
