﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public interface IRootContext : ISpawnerContext, ISenderContext, IStopperContext
    {
    }
}