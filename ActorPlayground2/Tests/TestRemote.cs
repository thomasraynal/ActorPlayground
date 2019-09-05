using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    [TestFixture]
    public class TestRemote
    {
        [Test]
        public async Task ShouldEmitEventFromWorld()
        {
            IActor actorFactory() => new HelloActorHandleEvent();

            var world1 = World.Create<TestRegistry>();
            var process1 = world1.Spawn(actorFactory, "http://127.0.0.1:8080");

            var world2 = World.Create<TestRegistry>();
            var process2 = world1.Spawn(actorFactory, "http://127.0.0.1:8181");

            world1.Emit("http://127.0.0.1:8181", new Hello(process1.Configuration.Id.Value));

            await Task.Delay(10);

            var actor = process2.Actor as HelloActorHandleEvent;

            Assert.IsNotNull(actor);

            Assert.AreEqual(1, actor.Received.Count);

            world1.Dispose();
            world2.Dispose();

        }

        [Test]
        public async Task ShouldEmitEventFromActor()
        {
            IActor actorFactory() => new HelloActorHandleEvent();

            var world1 = World.Create<TestRegistry>();
            var process1 = world1.Spawn(actorFactory, "http://127.0.0.1:8080");

            var world2 = World.Create<TestRegistry>();
            var process2 = world1.Spawn(actorFactory, "http://127.0.0.1:8181");

            process1.Emit("http://127.0.0.1:8181", new Hello(process1.Configuration.Id.Value));

            await Task.Delay(10);

            var actor = process2.Actor as HelloActorHandleEvent;

            Assert.IsNotNull(actor);

            Assert.AreEqual(1, actor.Received.Count);

            world1.Dispose();
            world2.Dispose();

        }

        [Test]
        public void ShouldNotBeAbleToSendMessageFromTranscientActorToRemote()
        {
            IActor actorFactory() => new HelloActorHandleCommand();

            var configuration1 = new RootRemoteConfiguration("http://127.0.0.1:8080");

            var world1 = World.Create<TestRegistry>(configuration1);
            var world2 = World.Create<TestRegistry>();
            var process = world1.Spawn(actorFactory, "http://127.0.0.1:8181");

            //send form world2 (transcient) to world1 first spawn (remote)
            Assert.ThrowsAsync<Exception>(async() =>
            {
                await world2.Send<DoSayHello>("http://127.0.0.1:8181", new SayHello(process.Configuration.Id.Value));
            });
            
        
            world1.Dispose();
            world2.Dispose();
        }

        [Test]
        public async Task ShouldExecuteCommandFromWorld()
        {

            IActor actorFactory() => new HelloActorHandleCommand();

            var configuration1 = new RootRemoteConfiguration("http://127.0.0.1:8080");
            var configuration2 = new RootRemoteConfiguration("http://127.0.0.1:8181");

            var world1 = World.Create<TestRegistry>(configuration1);
            var world2 = World.Create<TestRegistry>(configuration2);
            var process = world1.Spawn(actorFactory, "http://127.0.0.1:8282");

            //send from worl2 to world1 first spawn
            var commandResult = await world2.Send<DoSayHello>("http://127.0.0.1:8282", new SayHello(process.Configuration.Id.Value), TimeSpan.FromSeconds(2));

            await Task.Delay(10);

            Assert.AreEqual("ok", commandResult.Who);

            world1.Dispose();
            world2.Dispose();
        }

        [Test]
        public async Task ShouldExecuteCommandFromActor()
        {

            IActor actorFactory() => new HelloActorHandleCommand();

            var configuration1 = new RootRemoteConfiguration("http://127.0.0.1:8080");
            var configuration2 = new RootRemoteConfiguration("http://127.0.0.1:8181");

            var world1 = World.Create<TestRegistry>(configuration1);
            var world2 = World.Create<TestRegistry>(configuration2);

            var process1 = world1.Spawn(actorFactory, "http://127.0.0.1:8282");
            var process2 = world2.Spawn(actorFactory, "http://127.0.0.1:8383");

            //send from world1 spawn to world2 spawn
            var commandResult = await process1.Send<DoSayHello>("http://127.0.0.1:8383", new SayHello(process2.Configuration.Id.Value), TimeSpan.FromSeconds(2));

            await Task.Delay(10);

            Assert.AreEqual("ok", commandResult.Who);

            world1.Dispose();
            world2.Dispose();
        }



        [Test]
        public async Task ShouldApplySupervisionStrategy()
        {
            IActor actorFactory() => new FaultyActor();

            var world1 = World.Create<TestRegistry>();
            var process1 = world1.Spawn(actorFactory, "http://localhost:8080");

            var world2 = World.Create<TestRegistry>();
            var process2 = world2.Spawn(actorFactory, "http://localhost:8181");

            var actor = process1.Actor as FaultyActor;

            Assert.IsNotNull(actor);

            var idBefore = actor.Id;

            world2.Emit("http://localhost:8080", new Hello(process2.Configuration.Id.Value));

            await Task.Delay(10);

            actor = process1.Actor as FaultyActor;

            var idAfter = actor.Id;

            Assert.IsNotNull(actor);

            Assert.AreNotEqual(idBefore, idAfter);

            world1.Dispose();
            world2.Dispose();

        }

    }
}

