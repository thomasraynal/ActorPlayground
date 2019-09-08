using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public interface ICanPost : IHasId
    {
        void Post(IEvent msg, ICanPost sender);
    }
}
