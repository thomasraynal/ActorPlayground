using System;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorRegistry: IActor, IDisposable
    {
        IActorProcess Add(Func<IActor> actorFactory, ActorType type, ICanPost parent);
        IActorProcess Add(Func<IActor> actorFactory, string adress, ActorType type, ICanPost parent);
        IActorProcess Get(string id);
        void Remove(string id);
    }
}