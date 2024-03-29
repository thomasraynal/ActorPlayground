﻿using Microsoft.Extensions.Logging;
using Orleans.Providers.Streams.Common;
using Orleans.Streams;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ActorPlayground.Orleans.Basics.EventStore
{

    public class EventStoreQueueCache :  IQueueCache
    {
        private readonly ConcurrentDictionary<(Guid, string), ConcurrentQueue<EventStoreBatchContainer>> _cache;
        private readonly ConcurrentQueue<IBatchContainer> _itemsToPurge;
        private readonly int _maxCacheSize;
        private int _numItemsInCache;

        public EventStoreQueueCache(int cacheSize)
        {
            _maxCacheSize = cacheSize;
            _cache = new ConcurrentDictionary<(Guid, string), ConcurrentQueue<EventStoreBatchContainer>>();
            _itemsToPurge = new ConcurrentQueue<IBatchContainer>();
        }

        public int GetMaxAddCount()
        {
            return Math.Max(1, _maxCacheSize - _numItemsInCache);
        }

        public bool IsUnderPressure()
        {
            return _numItemsInCache >= _maxCacheSize;
        }

        public void AddToCache(IList<IBatchContainer> messages)
        {
            foreach (var message in messages)
            {
                var key = (message.StreamGuid, message.StreamNamespace);
                _cache.GetOrAdd(key, new ConcurrentQueue<EventStoreBatchContainer>()).Enqueue((EventStoreBatchContainer)message);
            }

            Interlocked.Add(ref _numItemsInCache, messages.Count);
        }

        public bool TryPurgeFromCache(out IList<IBatchContainer> purgedItems)
        {
            if (_itemsToPurge.IsEmpty)
            {
                purgedItems = null;
            }
            else
            {
                purgedItems = new List<IBatchContainer>();

                while (_itemsToPurge.TryDequeue(out IBatchContainer batchContainer))
                {
                    purgedItems.Add(batchContainer);
                }

                Interlocked.Add(ref _numItemsInCache, -(purgedItems).Count);
            }

            return purgedItems?.Count > 0;
        }

        public IQueueCacheCursor GetCacheCursor(IStreamIdentity streamIdentity, StreamSequenceToken token)
        {
    
            return new ConcurrentQueueCacheCursor(() =>
            {
                if (_cache.TryGetValue((streamIdentity.Guid, streamIdentity.Namespace), out ConcurrentQueue<EventStoreBatchContainer> concurrentQueue))
                {
                    if (concurrentQueue.TryDequeue(out EventStoreBatchContainer result))
                        return result;
                }
                return null;
            },
            item =>
            {
                if (item == null)
                    return;

                _itemsToPurge.Enqueue(item);
            });
        }
    }
}
