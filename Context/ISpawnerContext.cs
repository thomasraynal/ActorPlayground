using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{
    public interface ISpawnerContext
    {

        PID Spawn(Props props);
        PID SpawnNamed(Props props, string name);
        PID SpawnPrefix(Props props, string prefix);
    }
}
