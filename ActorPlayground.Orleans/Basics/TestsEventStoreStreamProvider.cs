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
    [Serializable]
    public class ChangeCcyPairPriceWithVersionId : ChangeCcyPairPrice, IEventWithVersionId
    {
        public ChangeCcyPairPriceWithVersionId()
        {
        }

        public ChangeCcyPairPriceWithVersionId(string ccyPair, string market, double ask, double bid) : base(ccyPair, market, ask, bid)
        {
        }

        public long Version { get; set; }
    }

    [TestFixture]
    public class TestsEventStoreStreamProvider
    {
        private const string serviceId = "OrleansCcyPairs";
        private const string euroDolStream = "EUR/USD";
        private const string euroJpyStream = "EUR/JPY";
        private const string harmonyMarket = "Harmony";
        private const string fxConnectMarket = "fxConnect";
        private readonly List<string> groups = new List<string>() { harmonyMarket, fxConnectMarket };

        private EmbeddedEventStoreFixture _embeddedEventStore;
        private IEventStoreConnection _connection;

        [OneTimeSetUp]
        public async Task SetupFixture()
        {

            _embeddedEventStore = new EmbeddedEventStoreFixture();
            await _embeddedEventStore.Initialize();

            await Task.Delay(5000);

            var userCredentials = new UserCredentials("admin", "changeit");
            var connectionSettings = ConnectionSettings.Create().WithConnectionTimeoutOf(TimeSpan.FromSeconds(10)).SetDefaultUserCredentials(userCredentials);

            _connection = EventStoreConnection.Create(connectionSettings, new Uri(EventStoreRepositoryConfiguration.Default.ConnectionString));

            await _connection.ConnectAsync();

            await Task.Delay(200);

            await CreateSubscription(_connection, euroDolStream, euroDolStream);

            await Task.Delay(200);

            await CreateSubscription(_connection, euroJpyStream, euroJpyStream);


        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            _connection.Close();

            await _embeddedEventStore.Dispose();
        }

        private async Task CreateSubscription(IEventStoreConnection connection, string stream, string group)
        {
            var persistentSubscriptionSettings = PersistentSubscriptionSettings.Create().DoNotResolveLinkTos().StartFromCurrent();
            var userCredentials = new UserCredentials("admin", "changeit");

            await connection.CreatePersistentSubscriptionAsync(stream, group, persistentSubscriptionSettings, userCredentials);
        
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
                                    services.AddSingleton<IEventStoreStreamProviderConfiguration>(new EventStoreStreamProviderConfiguration(groups));
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
            Task Publish(ChangeCcyPairPriceWithVersionId changeCcyPairPrice);
            Task<IEnumerable<IEventWithVersionId>> GetEvents();
        }

        public class TestObserver : Grain, IAsyncObserver<IEventWithVersionId>, ITestObserver
        {
            public List<IEventWithVersionId> Events = new List<IEventWithVersionId>();
            private IStreamProvider _streamProvider;
            private IAsyncStream<IEventWithVersionId> _euroDolStream;

            public async override Task OnActivateAsync()
            {
                _streamProvider = this.GetStreamProvider(euroDolStream);

                _euroDolStream = _streamProvider.GetStream<IEventWithVersionId>(Guid.Empty, euroDolStream);

               var subscription = await _euroDolStream.SubscribeAsync(this);

            }

            public Task<IEnumerable<IEventWithVersionId>> GetEvents()
            {
                return Task.FromResult(Events.AsEnumerable());
            }

            public async Task Publish(ChangeCcyPairPriceWithVersionId changeCcyPairPrice)
            {
                await _euroDolStream.OnNextAsync(changeCcyPairPrice);
            }

            public Task OnCompletedAsync()
            {
                return Task.CompletedTask;
            }

            public Task OnErrorAsync(Exception ex)
            {
                return Task.CompletedTask;
            }

            public Task OnNextAsync(IEventWithVersionId item, StreamSequenceToken token = null)
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

            await observer.Publish(new ChangeCcyPairPriceWithVersionId("EUR/USD", "Harmony", 1.32, 1.34));
            await observer.Publish(new ChangeCcyPairPriceWithVersionId("EUR/USD", "Harmony", 1.33, 1.34));
            await observer.Publish(new ChangeCcyPairPriceWithVersionId("EUR/USD", "Harmony", 1.31, 1.34));
            await observer.Publish(new ChangeCcyPairPriceWithVersionId("EUR/USD", "Harmony", 1.35, 1.36));

            await Task.Delay(500);

            var events = await observer.GetEvents();

            Assert.AreEqual(4, events.Count());

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();
        }

    }
}
