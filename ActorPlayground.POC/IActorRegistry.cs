namespace ActorPlayground.POC
{
    public interface IActorRegistry
    {
        ActorProcess Add(IActor actor);
        ActorProcess Get(string id);
        void Remove(ActorProcess id);
    }
}