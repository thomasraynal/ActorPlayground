﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICommand : IEvent
    {
        Guid CommandId { get; set; }
    }
}
