using Microsoft.Extensions.Logging;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public class EventStoreAdapterReceiver : IQueueAdapterReceiver
    {
        private readonly IEventStoreRepositoryConfiguration _repositoryConfiguration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly QueueId _queueId;
        private readonly string _providerName;

        private EventStoreRepository _eventStoreRepository;

        public EventStoreAdapterReceiver(IEventStoreRepositoryConfiguration repositoryConfiguration, ILoggerFactory loggerFactory, QueueId queueId, string providerName)
        {
            _repositoryConfiguration = repositoryConfiguration;
            _loggerFactory = loggerFactory;
            _queueId = queueId;
            _providerName = providerName;
        }

        public static IQueueAdapterReceiver Create(IEventStoreRepositoryConfiguration repositoryConfiguration, ILoggerFactory loggerFactory, QueueId queueId, string providerName)
        {
            return new EventStoreAdapterReceiver(repositoryConfiguration, loggerFactory, queueId, providerName);
        }

        public async Task<IList<IBatchContainer>> GetQueueMessagesAsync(int maxCount)
        {
            throw new NotImplementedException();

            //return await Task.Run(() =>
            //{

            //    List<IBatchContainer> batches = null;
            //    int count = 0;

            //    while (!_shutdownRequested)
            //    {
            //        if (count == maxCount)
            //            return batches;

            //        if (!IsConnected())
            //            Connect();

            //        var result = _model.BasicGet(_config.Queue, false);

            //        if (result == null)
            //            return batches;

            //        if (batches == null)
            //            batches = new List<IBatchContainer>();

            //        batches.Add(CreateContainer(result));

            //        count++;
            //    }

            //    return null;


            //});
        }

        public async Task Initialize(TimeSpan timeout)
        {
            _eventStoreRepository = EventStoreRepository.Create(_repositoryConfiguration);

            await _eventStoreRepository.Connect(TimeSpan.FromSeconds(5));

        }

        public Task MessagesDeliveredAsync(IList<IBatchContainer> messages)
        {
            throw new NotImplementedException();
        }

        public Task Shutdown(TimeSpan timeout)
        {
            _eventStoreRepository.Dispose();

            return Task.CompletedTask;
        }
    }
}
