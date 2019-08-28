using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public static class Extensions
    {
        public static void Stop(this IEnumerable<PID> self)
        {
            foreach (var pid in self)
            {
                RootContext.Empty.Stop(pid);
            }
        }

        public static void SendSystemNessage(this IEnumerable<PID> self, object message)
        {
            foreach (var pid in self)
            {
                pid.SendSystemMessage(message);
            }
        }

        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> self, out TKey key, out TValue value)
        {
            key = self.Key;
            value = self.Value;
        }
    }
}
