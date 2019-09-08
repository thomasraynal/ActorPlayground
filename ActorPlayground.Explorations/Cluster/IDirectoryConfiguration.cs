using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Explorations
{
    public interface IDirectoryConfiguration
    {
        TimeSpan MemberTtl { get; }
    }
}
