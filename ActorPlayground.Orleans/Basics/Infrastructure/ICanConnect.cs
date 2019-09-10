using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public interface ICanConnect
    {
        Task Connect(string provider);
        Task Disconnect();
    }
}
