using ActorPlayground.POC.Remote;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class RemoteWriter : IWriter
    {

        private readonly IActorRegistry _registry;
        private readonly ActorId _sender;
     //   private readonly Dictionary<string, ActorEndpoint> _endpointCache;

        public RemoteWriter(ActorId sender, IActorRegistry registry)
        {
            _registry = registry;
            _sender = sender;
          //  _endpointCache = new Dictionary<string, ActorEndpoint>();
        }

        private void EnsureEndpointExist(string targetId)
        {
          //  if (_endpointCache.ContainsKey(targetId)) return;


          //  _endpointCache.Add(targetId, new RemoteWriter())

        }

        public void Emit(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Emit(string targetId, IMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<T> Send<T>(string targetId, IMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<T> Send<T>(string targetId, IMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<T> Send<T>(string targetId, IMessage message, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}
