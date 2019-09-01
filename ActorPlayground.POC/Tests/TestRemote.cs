using ActorPlayground.Remote;
using Grpc.Core;
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
        public async Task ShouldEmitEvent()
        {

            var world = Factory.Create<TestRegistry>();

            IActor actorFactory() => new HelloActor();
            var process = world.Spawn(actorFactory, "http://localhost:8080");

            world.Emit(process.Configuration.Id.Value, new Hello(process.Configuration.Id.Value));

            await Task.Delay(10);

            var actor = process.Actor as HelloActor;

            Assert.IsNotNull(actor);

            Assert.AreEqual(1, actor.Received.Count);

            world.Dispose();

        }
    }
}
