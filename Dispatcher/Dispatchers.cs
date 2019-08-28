using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground
{

    public static class Dispatchers
    {
        public static ThreadPoolDispatcher DefaultDispatcher { get; } = new ThreadPoolDispatcher();
        public static SynchronousDispatcher SynchronousDispatcher { get; } = new SynchronousDispatcher();
    }

}
