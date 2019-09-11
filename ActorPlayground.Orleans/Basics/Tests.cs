using NUnit.Framework;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Providers;
using System.Linq;
using Orleans.Streams;

namespace ActorPlayground.Orleans.Basics
{
    [TestFixture]
    public class Tests
    {
        private const string serviceId = "OrleansCcyPairs";
        private EmbeddedEventStoreFixture _embeddedEventStore;

        [OneTimeSetUp]
        public async Task SetupFixture()
        {
            _embeddedEventStore = new EmbeddedEventStoreFixture();
            await _embeddedEventStore.Initialize();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _embeddedEventStore.Dispose();
        }

        private async Task<ISiloHost> CreateSilo(CancellationToken cancel)
        {

            var builder = new SiloHostBuilder()
                                .AddMemoryStreams<DefaultMemoryMessageBodySerializer>("CcyPairStream")
                                .AddMemoryGrainStorage("CcyPairStorage")
                                .AddMemoryGrainStorage("AsyncStreamHandlerStorage")
                                .AddMemoryGrainStorage("PubSubStore")
                                .AddLogStorageBasedLogConsistencyProvider("CcyPairEventStore")
                                .UseLocalhostClustering()
                                .Configure<ClusterOptions>(options =>
                                {
                                    options.ClusterId = "dev";
                                    options.ServiceId = serviceId;
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

        [Test]
        public async Task ShouldCreateAndCallGrain()
        {
            var cancel = new CancellationTokenSource();

            var silo = await CreateSilo(cancel.Token);
            var client = await GetClient();

            var eurodol = client.GetGrain<ICcyPairGrain>("EUR/USD");
            var isActive = await eurodol.GetIsActive();
            Assert.IsFalse(isActive);

            await eurodol.Activate();
            isActive = await eurodol.GetIsActive();
            Assert.IsTrue(isActive);

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();
        }

        [Test]
        public async Task ShouldDesactivateAndReactivateSubscription()
        {
            var cancel = new CancellationTokenSource();
            var silo = await CreateSilo(cancel.Token);
            var client = await GetClient();

            var fxConnect = client.GetGrain<IMarketGrain>("FxConnect");
            await fxConnect.Connect("CcyPairStream");

            var feedId = Guid.NewGuid();
            var feed = client.GetGrain<IFxFeedGrain<CcyPairChanged>>(feedId);
            await feed.Connect("CcyPairStream");
            await feed.Subscribe("EUR/USD");

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            var consumedEvents = await feed.GetConsumedEvents();
            Assert.AreEqual(1, consumedEvents.Count());

            await feed.Desactivate();

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await feed.GetConsumedEvents();
            Assert.AreEqual(0, consumedEvents.Count());

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await feed.GetConsumedEvents();
            Assert.AreEqual(1, consumedEvents.Count());

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();
        }

        [Test]
        public async Task ShouldStreamCcyPair()
        {
            var cancel = new CancellationTokenSource();
            var silo = await CreateSilo(cancel.Token);
            var client = await GetClient();

            var fxConnect = client.GetGrain<IMarketGrain>("FxConnect");
            await fxConnect.Connect("CcyPairStream");

            var feed1 = client.GetGrain<IFxFeedGrain<CcyPairChanged>>(Guid.NewGuid());
            await feed1.Connect("CcyPairStream");
            await feed1.Subscribe("EUR/USD");

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            var consumedEvents = await feed1.GetConsumedEvents();
            Assert.AreEqual(1, consumedEvents.Count());

            await fxConnect.Tick("EUR/CAD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await feed1.GetConsumedEvents();
            Assert.AreEqual(1, consumedEvents.Count());

            await feed1.Subscribe("EUR/CAD");
            await fxConnect.Tick("EUR/CAD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await feed1.GetConsumedEvents();
            Assert.AreEqual(2, consumedEvents.Count());

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();

        }

        [Test]
        public async Task ShouldSubscribeToCcyPair()
        {
            var cancel = new CancellationTokenSource();
            var silo = await CreateSilo(cancel.Token);
            var client = await GetClient();

            var euroDol = client.GetGrain<ICcyPairGrain>("EUR/USD");
            var eurJpy = client.GetGrain<ICcyPairGrain>("EUR/JPY");

            var fxConnect = client.GetGrain<IMarketGrain>("FxConnect");
            await fxConnect.Connect("CcyPairStream");
            var harmony = client.GetGrain<IMarketGrain>("Harmony");
            await harmony.Connect("CcyPairStream");

            var feed1 = client.GetGrain<IFxFeedGrain<CcyPairChanged>>(Guid.NewGuid());
            await feed1.Connect("CcyPairStream");
            await feed1.Subscribe("EUR/USD");
            await feed1.Subscribe("EUR/JPY");

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await harmony.Tick("EUR/USD", 1.33, 1.34);
            await fxConnect.Tick("EUR/USD", 1.34, 1.35);

            await fxConnect.Tick("EUR/JPY", 117.32, 117.34);
            await harmony.Tick("EUR/JPY", 117.33, 117.34);
            await harmony.Tick("EUR/JPY", 117.34, 117.35);

            await Task.Delay(2000);

            var currentEurJpyTick = await eurJpy.GetCurrentTick();

            Assert.AreEqual(117.34, currentEurJpyTick.bid);
            Assert.AreEqual(117.35, currentEurJpyTick.ask);

            var currentEuroDolTick = await euroDol.GetCurrentTick();

            Assert.AreEqual(1.34, currentEuroDolTick.bid);
            Assert.AreEqual(1.35, currentEuroDolTick.ask);

            var euroDolEvents = await euroDol.GetAppliedEvents();

            Assert.AreEqual(3, euroDolEvents.Count());

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();
        }

    }
}
