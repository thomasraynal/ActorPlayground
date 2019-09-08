using NUnit.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations.Tests
{
    public class TestRegistry : Registry
    {
        public TestRegistry()
        {
            For<IActorRegistry>().Use<InMemoryActorRegistry>().Singleton();
            For<ISerializer>().Use<JsonNetSerializer>();
            For<ISupervisorStrategy>().Use<OneForOneStrategy>();
            For<IRoot>().Use<Root>();
            For<IActorProcess>().Use<ActorProcess>();
            For<IRemoteActorProcess>().Use<RemoteActorProcess>();
            For<IRemoteActorProxyProvider>().Use<RemoteActorProxyProvider>();
        }
    }

    public class Hello : IEvent
    {
        public string Who { get; }

        public Hello(string who)
        {
            Who = who;
        }
    }

    public class SayHello : ICommand
    {
        public string Who { get; }

        public Guid CommandId { get; set; }

        public SayHello(string who)
        {
            Who = who;
        }
    }

    public class DoSayHello : ICommandResult
    {
        public string Who { get; }

        public Guid CommandId { get; set; }

        public DoSayHello(string who)
        {
            Who = who;
        }
    }

    public class FaultyActor : IActor
    {
        public Guid Id = Guid.NewGuid();

        public Task Receive(IMessageContext context)
        {
            var msg = context.Message;
            if (msg is Hello r)
            {


                throw new Exception("boom");
            }
            return Task.CompletedTask;
        }
    }

    public class HelloActorHandleEvent : IActor
    {
        public HelloActorHandleEvent()
        {
        }

        public List<Hello> Received { get; } = new List<Hello>();

        public Task Receive(IMessageContext context)
        {
            var msg = context.Message;
            if (msg is Hello r)
            {
                Received.Add(r);
            }
            return Task.CompletedTask;
        }
    }

    public class HelloActorHandleCommand : IActor
    {
        public Task Receive(IMessageContext context)
        {
            var msg = context.Message;
            if (msg is SayHello r)
            {
                var response = new DoSayHello("ok")
                {
                    CommandId = r.CommandId
                };

                context.Respond(response);
            }

            return Task.CompletedTask;
        }
    }


    [TestFixture]
    public class TestLocal
    {

            [Test]
            public async Task ShouldEmitEvent()
            {

                var world = World.Create<TestRegistry>();

                IActor actorFactory() => new HelloActorHandleEvent();
                var process = world.Spawn(actorFactory);

                world.Emit(process.Configuration.Id.Value, new Hello(process.Configuration.Id.Value));

                await Task.Delay(10);

                var actor = process.Actor as HelloActorHandleEvent;

                Assert.IsNotNull(actor);

                Assert.AreEqual(1, actor.Received.Count);

                world.Dispose();

            }

            [Test]
            public async Task ShouldExecuteCommand()
            {

                var world = World.Create<TestRegistry>();

                IActor actor() => new HelloActorHandleCommand();
                var process = world.Spawn(actor);

                var result = await world.Send<DoSayHello>(process.Configuration.Id.Value, new SayHello("ProtoActor"), TimeSpan.FromSeconds(2));

                Assert.AreEqual("ok", result.Who);

                world.Dispose();
            }

            [Test]
            public async Task ShouldApplySupervisionStrategy()
            {

                var world = World.Create<TestRegistry>();

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
            public async Task ShouldSpawnNamedActor()
            {
                var world = World.Create<TestRegistry>();
                var name = "MyActor";

                IActor actorFactory() => new HelloActorHandleEvent();
                var process = world.SpawnNamed(actorFactory, name);

                Assert.AreEqual(name, process.Id.Value);

                world.Emit(name, new Hello(name));

                await Task.Delay(10);

                var actor = process.Actor as HelloActorHandleEvent;

                Assert.IsNotNull(actor);

                Assert.AreEqual(1, actor.Received.Count);

                world.Dispose();
            }

            [Test]
            public async Task ShouldSpawnChildActor()
            {

                var world = World.Create<TestRegistry>();

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

