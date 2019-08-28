using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public interface IMessageInvoker
    {
        Task InvokeSystemMessageAsync(object msg);
        Task InvokeUserMessageAsync(object msg);
        void EscalateFailure(Exception reason, object message);
    }
}
