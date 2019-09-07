using System;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public interface IActorRegistry : IActor, IDisposable
    {
        IActorProcess Add(Func<IActor> actorFactory, ICanPost parent);
        IActorProcess Add(Func<IActor> actorFactory, string adress, ICanPost parent);
        ICanPost Get(string id);
        void Remove(string id);
    }
}