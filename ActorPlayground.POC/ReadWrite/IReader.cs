﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IReader
    {
        void Post(IMessage message);
    }
}
