using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
{
    public interface IActor
    {
        Task ReceiveAsync(IContext context);
    }
}
