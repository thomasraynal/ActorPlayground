﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.POC
{
    public interface IContext
    {
        IEnumerable<IActorProcess> Children { get; }

        IMessage Message { get; }

        IActorRegistry Registry { get; }

        void Respond(IMessage message);
    }
}