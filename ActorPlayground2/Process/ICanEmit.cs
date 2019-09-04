using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICanEmit
    {
        void Emit(string targetId, IEvent message);
    }
}
