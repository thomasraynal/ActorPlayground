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

