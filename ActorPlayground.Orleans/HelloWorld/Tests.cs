using NUnit.Framework;
using Orleans;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace ActorPlayground.Orleans.HelloWorld
{
    public interface IPlayerGrain : IGrainWithGuidKey
    {
        Task<IGameGrain> GetCurrentGame();
        Task JoinGame(IGameGrain game);
        Task LeaveGame(IGameGrain game);
    }

    public class GameGrain : Grain, IGameGrain
    {

    }

    public class PlayerGrain : Grain, IPlayerGrain
    {
        private IGameGrain currentGame;


        public override void Participate(IGrainLifecycle lifecycle)
        {
            base.Participate(lifecycle);
            lifecycle.Subscribe(this.GetType().FullName, GrainLifecycleStage.Activate, (cancel) =>
            {
                return Task.CompletedTask;
            });
        }

        // Game the player is currently in. May be null.
        public Task<IGameGrain> GetCurrentGame()
        {
            return Task.FromResult(currentGame);
        }

        // Game grain calls this method to notify that the player has joined the game.
        public Task JoinGame(IGameGrain game)
        {
            currentGame = game;
            Console.WriteLine(
                "Player {0} joined game {1}",
                this.GetPrimaryKey(),
                game.GetPrimaryKey());

            return Task.CompletedTask;
        }

        // Game grain calls this method to notify that the player has left the game.
        public Task LeaveGame(IGameGrain game)
        {
            currentGame = null;
            Console.WriteLine(
                "Player {0} left game {1}",
                this.GetPrimaryKey(),
                game.GetPrimaryKey());

            return Task.CompletedTask;
        }
    }

    public interface IGameGrain : IGrainWithGuidKey
    {
    }

    public interface IHello : IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }

    public class HelloGrain : Grain, IHello
    {
        private readonly ILogger logger;



        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        Task<string> IHello.SayHello(string greeting)
        {
            logger.LogInformation($"\n SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"\n Client said: '{greeting}', so HelloGrain says: Hello!");
        }
    }

    [TestFixture]
    public class Tests
    {
        private async Task<IClusterClient> GetClient()
        {

            var client = new ClientBuilder()
                                        .UseLocalhostClustering()
                                        .Configure<ClusterOptions>(options =>
                                        {
                                            options.ClusterId = "dev";
                                            options.ServiceId = "OrleansBasics";
                                        })
                                        .ConfigureLogging(logging => logging.AddConsole())
                                        .Build();

            await client.Connect();
            return client;
        }

        private void CreateSilo(CancellationToken cancel)
        {
             Task.Run(async() =>
            {

                var builder = new SiloHostBuilder()
                                        .UseLocalhostClustering()
                                        .Configure<ClusterOptions>(options =>
                                        {
                                            options.ClusterId = "dev";
                                            options.ServiceId = "OrleansBasics";
                                        })
                                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(Assembly.GetExecutingAssembly()).WithReferences())
                                        .ConfigureLogging(logging => logging.AddConsole());

                var host = builder.Build();
                await host.StartAsync(cancel);

                Console.ReadLine();
        

            }, cancel);

        }


        [Test]
        public async Task ShouldCreateAndCallGrain()
        {
            var cancel = new CancellationTokenSource();

            CreateSilo(cancel.Token);

            var client = await GetClient();

            var friend = client.GetGrain<IHello>(0);
            var response = await friend.SayHello("Good morning, HelloGrain!");

            Assert.IsNotNull(response);

            cancel.Cancel();
            client.Dispose();
        }

    }
}
