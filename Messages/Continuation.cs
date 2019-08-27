using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public class Continuation : SystemMessage
    {
        public Continuation(Func<Task> fun, object message)
        {
            Action = fun ?? throw new ArgumentNullException(nameof(fun));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public Func<Task> Action { get; }
        public object Message { get; }
    }
}
