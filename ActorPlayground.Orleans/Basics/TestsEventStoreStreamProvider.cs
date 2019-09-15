using ActorPlayground.Orleans.Basics.EventStore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Streams;
using System.Linq;
using EventStore.ClientAPI;
using System.Net;
using EventStore.ClientAPI.SystemData;

namespace ActorPlayground.Orleans.Basics
{
    [TestFixture]
    public class TestsEventStoreStreamProvider
    {
        private const string serviceId = "OrleansCcyPairs";
        private const string euroDolStream = "EUR/USD";
        private const string euroJpyStream = "EUR/JPY";
        private const string harmonyMarket = "Harmony";
        private const string fxConnectMarket = "fxConnect";
        private EmbeddedEventStoreFixture _embeddedEventStore;

        [OneTimeSetUp]
        public async Task SetupFixture()
        {
            _embeddedEventStore = new EmbeddedEventStoreFixture();
            await _embeddedEventStore.Initialize();

            await CreateSubscription(euroDolStream, harmonyMarket);
            await CreateSubscription(euroDolStream, fxConnectMarket);
            await CreateSubscription(euroJpyStream, harmonyMarket);
            await CreateSubscription(euroJpyStream, fxConnectMarket);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _embeddedEventStore.Dispose();
        }

        private async Task CreateSubscription(string stream, string group)
        {
            var persistentSubscriptionSettings = PersistentSubscriptionSettings.Create().DoNotResolveLinkTos().StartFromCurrent();
            var connectionSettings = ConnectionSettings.Create();
            var userCredentials = new UserCredentials("admin", "changeit");

            using (var connection = EventStoreConnection.Create(connectionSettings, EventStoreRepositoryConfiguration.Default.ConnectionString))
            {
                await connection.CreatePersistentSubscriptionAsync(stream, group, persistentSubscriptionSettings, userCredentials);
            }

        }

        private async Task<ISiloHost> CreateSilo(CancellationToken cancel)
        {

            var builder = new SiloHostBuilder()
                                .AddMemoryStreams<DefaultMemoryMessageBodySerializer>("CcyPairStream")
                                .AddMemoryGrainStorage("CcyPairStorage")
                                .AddMemoryGrainStorage("AsyncStreamHandlerStorage")
                                .AddMemoryGrainStorage("PubSubStore")
                                .AddEventStorePersistentStream(euroDolStream)
                                .AddEventStorePersistentStream(euroJpyStream)
                                .UseLocalhostClustering()
                                .Configure<ClusterOptions>(options =>
                                {
                                    options.ClusterId = "dev";
                                    options.ServiceId = serviceId;
                                })
                                .ConfigureServices(services =>
                                {
                                    services.AddTransient<IEventStoreRepositoryConfiguration, EventStoreRepositoryConfiguration>();
                                })
                                .ConfigureApplicationParts(parts => parts.AddApplicationPart(Assembly.GetExecutingAssembly()).WithReferences())
                                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync(cancel);

            return host;

        }

        private async Task<IClusterClient> GetClient()
        {

            var client = new ClientBuilder()
                                        .UseLocalhostClustering()
                                        .AddEventStorePersistentStream(euroDolStream)
                                        .AddEventStorePersistentStream(euroJpyStream)
                                        .Configure<ClusterOptions>(options =>
                                        {
                                            options.ClusterId = "dev";
                                            options.ServiceId = serviceId;
                                        })
                                        .ConfigureLogging(logging => logging.AddConsole())
                                        .Build();

            await client.Connect();
            return client;
        }

        public interface ITestObserver : IGrainWithGuidKey
        {
            Task Publish();
            Task<int> GetEventCounts();
        }

        public class TestObserver : Grain, IAsyncObserver<IEvent>, ITestObserver
        {
            public List<IEvent> Events = new List<IEvent>();
            private IStreamProvider _streamProvider;
            private IAsyncStream<IEvent> _euroDolStream;

            public async override Task OnActivateAsync()
            {
                _streamProvider = this.GetStreamProvider(euroDolStream);

                _euroDolStream = _streamProvider.GetStream<IEvent>(Guid.Empty, "Harmony");

                var subscription = await _euroDolStream.SubscribeAsync(this);

            }

            public Task<int> GetEventCounts()
            {
                return Task.FromResult(Events.Count);
            }

            public async Task Publish()
            {
                await _euroDolStream.OnNextAsync(new ChangeCcyPairPrice("EUR/USD", "Harmony", 1.32, 1.34));
            }

            public Task OnCompletedAsync()
            {
                return Task.CompletedTask;
            }

            public Task OnErrorAsync(Exception ex)
            {
                return Task.CompletedTask;
            }

            public Task OnNextAsync(IEvent item, StreamSequenceToken token = null)
            {
                Events.Add(item);

                return Task.CompletedTask;
            }
        }


        [Test]
        public async Task ShouldWriteAndReadFromStream()
        {
            var cancel = new CancellationTokenSource();

            var silo = await CreateSilo(cancel.Token);
            var client = await GetClient();

            var observer = client.GetGrain<ITestObserver>(Guid.NewGuid());

            await observer.Publish();
            await observer.Publish();
            await observer.Publish();
            await observer.Publish();

            await Task.Delay(2000);

            var count = await observer.GetEventCounts();

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();
        }

    }
}
