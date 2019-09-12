using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreQueueAdapter : IQueueAdapter
    {
        private readonly IEventStoreRepositoryConfiguration _repositoryConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IStreamQueueMapper _streamQueueMapper;
        private readonly string _providerName;

        public EventStoreQueueAdapter(IEventStoreRepositoryConfiguration repositoryConfiguration, ILoggerFactory loggerFactory, string providerName, IStreamQueueMapper streamQueueMapper)
        {
            _repositoryConfiguration = repositoryConfiguration;
            _loggerFactory = loggerFactory;
            _streamQueueMapper = streamQueueMapper;
            _providerName = providerName;
        }

        public string Name { get; private set; }

        public bool IsRewindable => true;

        public StreamProviderDirection Direction => StreamProviderDirection.ReadWrite;

        public IQueueAdapterReceiver CreateReceiver(QueueId queueId)
        {
            return EventStoreAdapterReceiver.Create(_repositoryConfiguration, _loggerFactory, queueId, Name);
        }

        public async Task QueueMessageBatchAsync<T>(Guid streamGuid, string streamNamespace, IEnumerable<T> events, StreamSequenceToken token, Dictionary<string, object> requestContext)
        {
            await Task.Run(() =>
            {
                var queue = _streamQueueMapper.GetQueueForStream(streamGuid, streamNamespace);

                // TODO: Handle queues.

                //foreach (var e in events)
                //{
                //    var bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(e));
                //    _model.BasicPublish(_config.Exchange, _config.RoutingKey, null, bytes);
                //}

            });
        }
    }
}
