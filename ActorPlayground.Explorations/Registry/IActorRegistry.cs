using System;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public interface IActorRegistry : IActor, IDisposable
    {
        IActorProcess Add(Func<IActor> actorFactory, ICanPost parent);
        IActorProcess Add(Func<IActor> actorFactory, string adress, ICanPost parent);
        IActorProcess Add(Func<IActor> actorFactory, ICanPost parent, string name);
        IActorProcess Add(Func<IActor> actorFactory, string adress, ICanPost parent, string name);
        ICanPost Get(string id);
        void Remove(ActorId id);
    }
}