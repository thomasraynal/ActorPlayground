namespace ActorPlayground.POC
{
    public interface IActorRegistry
    {
        ActorProcess Add(IActor actor, ActorProcess parent);
        ActorProcess Get(string id);
        void Remove(string id);
    }
}