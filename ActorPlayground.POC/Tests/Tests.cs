using ActorPlayground.POC.Remote;
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
            For<IActorRegistry>().Use<InMemoryActorRegistry>().Singleton();
            For<ISerializer>().Use<JsonNetSerializer>();
            For<ISupervisorStrategy>().Use<OneForOneStrategy>();
            For<IWorld>().Use<World>();
            For<IActorProcess>().Use<ActorProcess>();
        }
    }

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
    public class TestPoc
    {

        [TestFixture]
        public class Tests
        {
            [Test]
            public async Task ShouldEmitEvent()
            {

                var world = Factory.Create<TestRegistry>();

                IActor actorFactory() => new HelloActor();
                var process = world.Spawn(actorFactory);

                world.Emit(process.Configuration.Id.Value, new Hello(process.Configuration.Id.Value));

                await Task.Delay(10);

                var actor = process.Actor as HelloActor;

                Assert.IsNotNull(actor);

                Assert.AreEqual(1, actor.Received.Count);

                world.Dispose();

            }

            [Test]
            public async Task ShouldExecuteCommand()
            {
     
                var world = Factory.Create<TestRegistry>();

                IActor actor() => new HelloActor2();
                var process = world.Spawn(actor);
          
                var result = await world.Send<Hello>(process.Configuration.Id.Value, new Hello("ProtoActor"), TimeSpan.FromSeconds(2));

                Assert.AreEqual("ok", result.Who);

                world.Dispose();
            }

            [Test]
            public async Task ShouldApplySupervisionStrategy()
            {
         
                var world = Factory.Create<TestRegistry>();

                IActor actorFactory() => new FaultyActor();
                var process = world.Spawn(actorFactory);

                var actor = process.Actor as FaultyActor;

                Assert.IsNotNull(actor);

                var idBefore = actor.Id;

                world.Emit(process.Configuration.Id.Value, new Hello(process.Configuration.Id.Value));

                await Task.Delay(10);

                actor = process.Actor as FaultyActor;

                var idAfter = actor.Id;

                Assert.IsNotNull(actor);

                Assert.AreNotEqual(idBefore, idAfter);

                world.Dispose();

            }

            [Test]
            public async Task ShouldSpawnChildActor()
            {

                var world = Factory.Create<TestRegistry>();

                IActor actorFactory() => new FaultyActor();
                var parent = world.Spawn(actorFactory);

                var child = parent.SpawnChild(actorFactory);
                var childActor = child.Actor as FaultyActor;

                Assert.IsNotNull(childActor);

                var idBefore = childActor.Id;

                world.Emit(parent.Configuration.Id.Value, new Hello(parent.Configuration.Id.Value));

                await Task.Delay(1000);

                childActor = child.Actor as FaultyActor;

                var idAfter = childActor.Id;

                Assert.IsNotNull(childActor);

                Assert.AreNotEqual(idBefore, idAfter);

                world.Dispose();


            }

        }
    }

}
