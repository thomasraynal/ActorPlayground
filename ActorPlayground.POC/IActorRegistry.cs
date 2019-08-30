using System;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorRegistry
    {
        IActorProcess AddTransient(Func<IActor> actorFactory, IActorProcess parent);
        IActorProcess Add(Func<IActor> actorFactory, string adress, IActorProcess parent);
        IActorProcess Get(string id);
        void Remove(string id);
    }
}