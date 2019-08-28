using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public interface IMailbox
    {
        IMessageInvoker Invoker { get; }
        IDispatcher Dispatcher { get; }
        void PostUserMessage(object msg);
        void PostSystemMessage(object msg);
        void RegisterHandlers(IMessageInvoker invoker, IDispatcher dispatcher);
        void Start();
    }
}
