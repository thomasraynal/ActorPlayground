using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public interface ICanEmit
    {
        void Emit(string targetId, IEvent message);
    }
}
