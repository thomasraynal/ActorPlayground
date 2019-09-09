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

namespace ActorPlayground.Orleans.Basics
{
    [TestFixture]
    public class Tests
    {
        private const string serviceId = "OrleansCcyPairs";

        private void CreateSilo(CancellationToken cancel)
        {
            Task.Run(async () =>
            {

                var builder = new SiloHostBuilder()
                                    .AddMemoryGrainStorage("CcyPairStorage")
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

                Console.ReadLine();


            }, cancel);
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

            CreateSilo(cancel.Token);

            var client = await GetClient();

            var eurodol = client.GetGrain<ICcyPairGrain>("EUR/USD");

            var isActive = await eurodol.GetIsActive();

            Assert.IsFalse(isActive);

            await eurodol.Activate();

            isActive = await eurodol.GetIsActive();

            Assert.IsTrue(isActive);

            cancel.Cancel();
            client.Dispose();
        }
    }
}
