using System;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorRegistry
    {
        IActorProcess Add(Func<IActor> actorFactory, IActorProcess parent);
        IActorProcess Get(string id);
        void Remove(string id);
    }
}