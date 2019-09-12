using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreQueueAdapter : IQueueAdapter
    {
        public string Name => throw new NotImplementedException();

        public bool IsRewindable => throw new NotImplementedException();

        public StreamProviderDirection Direction => throw new NotImplementedException();

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            throw new NotImplementedException();
        }

        public Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            throw new NotImplementedException();
        }
    }
}
