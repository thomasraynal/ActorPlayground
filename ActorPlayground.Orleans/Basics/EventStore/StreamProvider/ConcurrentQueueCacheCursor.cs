using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class ConcurrentQueueCacheCursor : IQueueCacheCursor, IDisposable
    {
        private readonly object syncRoot = new object();
        private readonly Func<EventStoreBatchContainer> _moveNext;
        private readonly Action<EventStoreBatchContainer> _purgeItem;

        private EventStoreBatchContainer _current;

        public ConcurrentQueueCacheCursor(Func<EventStoreBatchContainer> moveNext, Action<EventStoreBatchContainer> purgeItem)
        {
            _moveNext = moveNext ?? throw new ArgumentNullException(nameof(moveNext));
            _purgeItem = purgeItem ?? throw new ArgumentNullException(nameof(purgeItem));
        }

        public void Dispose()
        {
            lock (syncRoot)
            {
                _purgeItem(_current);
                _current = null;
            }
        }

        public IBatchContainer GetCurrent(out Exception exception)
        {
            exception = null;
            return _current;
        }

        public bool MoveNext()
        {
            lock (syncRoot)
            {
                _purgeItem(_current);
                _current = _moveNext();
            }
            return _current != null;
        }

        public void Refresh(StreamSequenceToken token)
        {
        }

        public void RecordDeliveryFailure()
        {
            if (_current == null)
                return;

            //_current.DeliveryFailure = true;
        }
    }
}
