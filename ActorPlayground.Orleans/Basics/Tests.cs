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

        private async Task<ISiloHost> CreateSilo(CancellationToken cancel)
        {

            var builder = new SiloHostBuilder()
                                .AddMemoryStreams<DefaultMemoryMessageBodySerializer>("CcyPairStream")
                                .AddMemoryGrainStorage("CcyPairStorage")
                                .AddMemoryGrainStorage("AsyncStreamHandlerStorage")
                                .AddMemoryGrainStorage("PubSubStore")
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

            var traderId = Guid.NewGuid();
            var trader = client.GetGrain<IFxFeedGrain<CcyPairChanged>>(traderId);
            await trader.Connect("CcyPairStream");
            await trader.Subscribe("EUR/USD");

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            var consumedEvents = await trader.GetConsumedEvents();
            Assert.AreEqual(1, consumedEvents.Count());

            await trader.Desactivate();

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await trader.GetConsumedEvents();
            Assert.AreEqual(0, consumedEvents.Count());

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await trader.GetConsumedEvents();
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

            var trader1 = client.GetGrain<IFxFeedGrain<CcyPairChanged>>(Guid.NewGuid());
            await trader1.Connect("CcyPairStream");
            await trader1.Subscribe("EUR/USD");

            await fxConnect.Tick("EUR/USD", 1.32, 1.34);
            await Task.Delay(200);
            var consumedEvents = await trader1.GetConsumedEvents();
            Assert.AreEqual(1, consumedEvents.Count());

            await fxConnect.Tick("EUR/CAD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await trader1.GetConsumedEvents();
            Assert.AreEqual(1, consumedEvents.Count());

            await trader1.Subscribe("EUR/CAD");
            await fxConnect.Tick("EUR/CAD", 1.32, 1.34);
            await Task.Delay(200);
            consumedEvents = await trader1.GetConsumedEvents();
            Assert.AreEqual(2, consumedEvents.Count());

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();

        }


    }
}
