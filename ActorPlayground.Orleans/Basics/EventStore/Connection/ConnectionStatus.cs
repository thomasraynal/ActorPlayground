namespace ActorPlayground.Orleans.Basics.EventStore
{
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Closed,
        ErrorOccurred,
        AuthenticationFailed
    }
}