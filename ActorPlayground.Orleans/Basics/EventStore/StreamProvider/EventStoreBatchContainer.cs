using EventStore.ClientAPI;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    [Serializable]
    public class EventStoreBatchContainer : IBatchContainer
    {
        public Guid StreamGuid { get; }
        public string StreamNamespace { get; }
        public StreamSequenceToken SequenceToken { get; }
        public IEvent Event { get; }

        public EventStoreBatchContainer(Guid streamGuid, string streamNamespace, StreamSequenceToken sequenceToken, IEvent @event)
        {
            Event = @event;
            StreamGuid = streamGuid;
            StreamNamespace = streamNamespace;
            SequenceToken = sequenceToken;
        }

        public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
        {
            //todo : agnostic eventstore repository
            yield return new Tuple<T, StreamSequenceToken>((T)Event, SequenceToken);
        }

        public bool ImportRequestContext()
        {
            return false;
        }

        public bool ShouldDeliver(IStreamIdentity stream, object filterData, StreamFilterPredicate shouldReceiveFunc)
        {
            return true;
        }
    }
}
