﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics
{
    public interface IEventWithVersionId : IHasVersionId, IEvent
    {
    }
}
