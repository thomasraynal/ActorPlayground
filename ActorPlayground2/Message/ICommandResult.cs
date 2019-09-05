using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICommandResult : IEvent
    {
        Guid CommandId { get; set; }
    }
}
