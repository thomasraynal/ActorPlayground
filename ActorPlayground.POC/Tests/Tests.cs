using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    [TestFixture]
    public class TestPoc
    {
        public class Hello
        {
            public string Who { get; }

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
                var cluster = new Cluster();
                var supervisor = new Supervisor(new OneForOneStrategy(cluster));
                var registry = new ActorRegistry(supervisor);

                cluster.Initialize(registry);

                var actor = new HelloActor();
                var process = cluster.Spawn(actor);

                cluster.Emit(process.Id, new Hello(process.Id));

                await Task.Delay(10);

                Assert.AreEqual(1, actor.Received.Count);

            }

            [Test]
            public async Task ShouldExecuteCommand()
            {
                var cluster = new Cluster();
                var supervisor = new Supervisor(new OneForOneStrategy(cluster));
                var registry = new ActorRegistry(supervisor);

                cluster.Initialize(registry);

                var actor = new HelloActor2();
                var process = cluster.Spawn(actor);
          
                var result = await cluster.Send<Hello>(process.Id, new Hello("ProtoActor"), TimeSpan.FromSeconds(1));

                Assert.AreEqual("ok", result.Who);
            }

            [Test]
            public async Task ShouldApplySupervisionStrategy()
            {
                var cluster = new Cluster();
                var supervisor = new Supervisor(new OneForOneStrategy(cluster));
                var registry = new ActorRegistry(supervisor);

                cluster.Initialize(registry);

                var actor = new FaultyActor();
                var process = cluster.Spawn(actor);

                cluster.Emit(process.Id, new Hello(process.Id));

                await Task.Delay(10);

             //   Assert.AreEqual(1, actor.Received.Count);

            }

            [Test]
            public void ShouldCreateChildActor()
            {

            }

        }
    }

}
