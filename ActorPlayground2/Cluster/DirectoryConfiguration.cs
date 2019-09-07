using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public class DirectoryConfiguration : IDirectoryConfiguration
    {
        public DirectoryConfiguration(TimeSpan memberTtl)
        {
            MemberTtl = memberTtl;
        }

        public TimeSpan MemberTtl { get; }
    }
}
