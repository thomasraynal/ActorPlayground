using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public interface IHasGroupId
    {
        string GroupId { get; }
    }
}
