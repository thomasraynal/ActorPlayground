﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface ICanPost : IHasId
    {
        void Post(IEvent msg, ICanPost sender);
    }
}
