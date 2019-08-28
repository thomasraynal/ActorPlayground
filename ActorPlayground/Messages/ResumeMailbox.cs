using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public class ResumeMailbox : SystemMessage
    {
        public static ResumeMailbox Instance => new ResumeMailbox();
    }
}
