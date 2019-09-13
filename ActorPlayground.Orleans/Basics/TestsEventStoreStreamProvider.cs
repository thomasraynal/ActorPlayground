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

namespace ActorPlayground.Orleans.Basics
{
    [TestFixture]
    public class TestsEventStoreStreamProvider
    {
        private const string serviceId = "OrleansCcyPairs";
        private EmbeddedEventStoreFixture _embeddedEventStore;

        [SetUp]
        public async Task SetupFixture()
        {
            //_embeddedEventStore = new EmbeddedEventStoreFixture();
            //await _embeddedEventStore.Initialize();
        }

        [TearDown]
        public async Task TearDown()
        {
            //await _embeddedEventStore.Dispose();
        }

        private async Task<ISiloHost> CreateSilo(CancellationToken cancel)
        {

            var builder = new SiloHostBuilder()
                                .AddMemoryStreams<DefaultMemoryMessageBodySerializer>("CcyPairStream")
                                .AddMemoryGrainStorage("CcyPairStorage")
                                .AddMemoryGrainStorage("AsyncStreamHandlerStorage")
                                .AddMemoryGrainStorage("PubSubStore")
                                .AddEventStoreStreamProvider("EventStoreStreamProvider")
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
                                        .AddEventStoreStreamProvider("EventStoreStreamProvider")
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


        public class TestObserver<T> : IAsyncObserver<T>
        {
            public List<T> Events = new List<T>();

            public Task OnCompletedAsync()
            {
                return Task.CompletedTask;
            }

            public Task OnErrorAsync(Exception ex)
            {
                return Task.CompletedTask;
            }

            public Task OnNextAsync(T item, StreamSequenceToken token = null)
            {
                Events.Add(item);

                return Task.CompletedTask;
            }
        }

        [Test]
        public async Task ShouldGetStreamProvider()
        {
            var cancel = new CancellationTokenSource();

            var silo = await CreateSilo(cancel.Token);
            var client = await GetClient();

            var observer = new TestObserver<IEvent>();

            var streamProvider = client.GetStreamProvider("EventStoreStreamProvider");
   
            var euroDolStream = await streamProvider.GetStream<IEvent>(Guid.Empty, "USD/EUR")
                                        .SubscribeAsync(observer);

            Assert.IsNotNull(euroDolStream);

            cancel.Cancel();
            await silo.StopAsync();
            client.Dispose();
        }

    }
}
