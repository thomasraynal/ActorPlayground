﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IClusterConfiguration : IRootRemoteConfiguration
    {
        string Name { get; }
    }
}
