using NUnit.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class TestRegistry : Registry
    {
        public TestRegistry()
        {
            For<IActorRegistry>().Use<LocalActorRegistry>().Singleton();
            For<ISerializer>().Use<JsonNetSerializer>();
            For<ISupervisorStrategy>().Use<OneForOneStrategy>();
            For<IRoot>().Use<Root>();
            For<IActorProcess>().Use<ActorProcess>();
        }
    }

    [TestFixture]
    public class TestPoc
    {
        public class Hello : IMessage
        {
            public string Who { get; }

            public bool IsSystemMessage => false;

            public Hello(string who)
            {
                Who = who;
            }
        }

        public class FaultyActor : IActor
        {
            public Guid Id = Guid.NewGuid();

            public Task Receive(IContext context)
            {
                var msg = context.Message;
                if (msg is Hello r)
                {
                    throw new Exception("boom");
                }
                return Task.CompletedTask;
            }
        }

        public class HelloActor : IActor
        {
            public HelloActor()
            {
            }

            public List<Hello> Received { get; } = new List<Hello>();

            public Task Receive(IContext context)
            {
                var msg = context.Message;
                if (msg is Hello r)
                {
                    Received.Add(r);
                }
                return Task.CompletedTask;
            }
        }

        public class HelloActor2 : IActor
        {
            public Task Receive(IContext context)
            {
                var msg = context.Message;
                if (msg is Hello r)
                {
                    context.Respond(new Hello("ok"));
                }

                return Task.CompletedTask;
            }
        }

        [TestFixture]
        public class Tests
        {
            [Test]
            public async Task ShouldEmitEvent()
            {

                var configuration = new RootConfiguration("http://localhost:9090");

                var cluster = Factory.Create<TestRegistry>(configuration);

                IActor actorFactory() => new HelloActor();
                var process = cluster.Spawn(actorFactory, "http://localhost:8080");

                cluster.Emit(process.Configuration.Id.Value, new Hello(process.Configuration.Id.Value));

                await Task.Delay(10);

                var actor = process.Actor as HelloActor;

                Assert.IsNotNull(actor);

                Assert.AreEqual(1, actor.Received.Count);

            }

            [Test]
            public async Task ShouldExecuteCommand()
            {
                var configuration = new RootConfiguration("http://localhost:9090");

                var cluster = Factory.Create<TestRegistry>(configuration);

                IActor actor() => new HelloActor2();
                var process = cluster.Spawn(actor, "http://localhost:8080");
          
                var result = await cluster.Send<Hello>(process.Configuration.Id.Value, new Hello("ProtoActor"), TimeSpan.FromSeconds(2));

                Assert.AreEqual("ok", result.Who);
            }

            [Test]
            public async Task ShouldApplySupervisionStrategy()
            {
                var configuration = new RootConfiguration("http://localhost:9090");

                var cluster = Factory.Create<TestRegistry>(configuration);

                IActor actorFactory() => new FaultyActor();
                var process = cluster.Spawn(actorFactory, "http://localhost:8080");

                var actor = process.Actor as FaultyActor;

                Assert.IsNotNull(actor);

                var idBefore = actor.Id;

                cluster.Emit(process.Configuration.Id.Value, new Hello(process.Configuration.Id.Value));

                await Task.Delay(10);

                actor = process.Actor as FaultyActor;

                var idAfter = actor.Id;

                Assert.IsNotNull(actor);

                Assert.AreNotEqual(idBefore, idAfter);

            }

            [Test]
            public async Task ShouldSpawnChildActor()
            {
                var configuration = new RootConfiguration("http://localhost:9090");

                var cluster = Factory.Create<TestRegistry>(configuration);

                IActor actorFactory() => new FaultyActor();
                var parent = cluster.Spawn(actorFactory, "http://localhost:8080");

                var child = parent.SpawnChild(actorFactory);
                var childActor = child.Actor as FaultyActor;

                Assert.IsNotNull(childActor);

                var idBefore = childActor.Id;

                cluster.Emit(parent.Configuration.Id.Value, new Hello(parent.Configuration.Id.Value));

                await Task.Delay(1000);

                childActor = child.Actor as FaultyActor;

                var idAfter = childActor.Id;

                Assert.IsNotNull(childActor);

                Assert.AreNotEqual(idBefore, idAfter);


            }

        }
    }

}
