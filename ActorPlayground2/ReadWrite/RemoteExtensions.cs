using ActorPlayground.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC.Remote
{
    public static class RemoteExtensions
    {
        public static ActorId ToActorId(this Pid pid)
        {
            return new ActorId(pid.Id, pid.Address, ActorType.Remote);
        }

        public static Pid ToPid(this ActorId actorId)
        {
            return new Pid() { Address = actorId.Adress, Id = actorId.Value };
        }
    }
}
