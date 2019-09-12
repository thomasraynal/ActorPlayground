using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    [Serializable]
    public class EventStoreBatchContainer : IBatchContainer
    {
        public Guid StreamGuid => throw new NotImplementedException();

        public string StreamNamespace => throw new NotImplementedException();

        public StreamSequenceToken SequenceToken => throw new NotImplementedException();

        public IEnumerable<Tuple<T, StreamSequenceToken>> GetEvents<T>()
        {
            throw new NotImplementedException();
        }

        public bool ImportRequestContext()
        {
            throw new NotImplementedException();
        }

        public bool ShouldDeliver(IStreamIdentity stream, object filterData, StreamFilterPredicate shouldReceiveFunc)
        {
            throw new NotImplementedException();
        }
    }
}
