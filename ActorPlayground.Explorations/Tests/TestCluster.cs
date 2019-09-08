using NUnit.Framework;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations.Tests
{

    public class TestClusterRegistry : Registry
    {
        public TestClusterRegistry()
        {
            For<IClusterRegistry>().Use<ClusterRegistry>().Singleton();
            Forward<IClusterRegistry, IActorRegistry>();
            For<ISerializer>().Use<JsonNetSerializer>();
            For<ISupervisorStrategy>().Use<OneForOneStrategy>();
            For<ICluster>().Use<ClusterRoot>();
            For<IDirectoryConfiguration>().Use<DirectoryConfiguration>();
            For<IDirectory>().Use<InMemoryClusterDirectory>();
            For<IActorProcess>().Use<ActorProcess>();
            For<IRemoteActorProcess>().Use<RemoteActorProcess>();
            For<IRemoteActorProxyProvider>().Use<RemoteActorProxyProvider>();
        }
    }

    [TestFixture]
    public class TestCluster
    {
        [Test]
        public void ShouldCreateCluster()
        {
            IActor actorFactory() => new HelloActorHandleEvent();

            var clusterConfiguration = new ClusterConfiguration("TestCluster", "http://127.0.0.1:8080", actorFactory);
            var directoryConfiguration = new DirectoryConfiguration(TimeSpan.FromSeconds(30));

            var cluster = Cluster.Create<TestClusterRegistry>(clusterConfiguration, directoryConfiguration);

            Assert.IsNotNull(cluster);

            cluster.Dispose();

        }

        [Test]
        public async Task ShouldEmitEventFromCluster()
        {
            IActor actorFactory() => new HelloActorHandleEvent();

            var clusterConfiguration1 = new ClusterConfiguration("TestCluster1", "http://127.0.0.1:8080", actorFactory);
            var clusterConfiguration2 = new ClusterConfiguration("TestCluster2", "http://127.0.0.1:8181", actorFactory);
            var directoryConfiguration = new DirectoryConfiguration(TimeSpan.FromSeconds(30));

            var cluster1 = Cluster.Create<TestClusterRegistry>(clusterConfiguration1, directoryConfiguration);
            var process1 = cluster1.Spawn(actorFactory, "http://127.0.0.1:8282");

            var cluster2 = Cluster.Create<TestClusterRegistry>(clusterConfiguration2, directoryConfiguration);
            var process2 = cluster1.Spawn(actorFactory, "http://127.0.0.1:8383");

            cluster1.Emit("http://127.0.0.1:8383", new Hello(process1.Configuration.Id.Value));

            await Task.Delay(10);

            var actor = process2.Actor as HelloActorHandleEvent;

            Assert.IsNotNull(actor);

            Assert.AreEqual(1, actor.Received.Count);

            cluster1.Dispose();
            cluster2.Dispose();

        }


        [Test]
        public async Task ShouldJoinCluster()
        {
            //IActor actorFactory() => new HelloActorHandleEvent();


            //var clusterConfiguration = new ClusterConfiguration("TestCluster2", "http://127.0.0.1:8080", actorFactory);
            //var directoryConfiguration = new DirectoryConfiguration(TimeSpan.FromSeconds(30));
            //var cluster = Cluster.Create<TestClusterRegistry>(clusterConfiguration, directoryConfiguration);

            //var configuration = new RootRemoteConfiguration("http://127.0.0.1:8181", actorFactory);

            //var world = World.Create<TestRegistry>(configuration);

          //  Cluster.Join("http://127.0.0.1:8080", world.Id);


        }

        [Test]
        public async Task ShouldResolveActorName()
        {

        }


        [Test]
        public async Task ShouldPartFromCluster()
        {

        }

    }
}
