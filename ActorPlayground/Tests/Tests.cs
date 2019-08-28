using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground
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
        public Task ReceiveAsync(IContext context)
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

        public Task ReceiveAsync(IContext context)
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
        public Task ReceiveAsync(IContext context)
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
        public async Task TesEmitEvent()
        {
            var context = new RootContext();
            var props = Props.FromProducer(() => new HelloActor());
            var pid = context.Spawn(props);

            context.Send(pid, new Hello("ProtoActor"));

            await Task.Delay(100);

            var process = ProcessRegistry.Instance.Get(pid) as ActorProcess;

            Assert.IsNotNull(process);

            var actorContext = process.Mailbox.Invoker as ActorContext;

            Assert.IsNotNull(actorContext);

            var actor = actorContext.Actor as HelloActor;

            Assert.IsNotNull(actor);

            Assert.AreEqual(1, actor.Received.Count);
        }

        [Test]
        public async Task TestExecuteCommand()
        {
            var context = new RootContext();
            var props = Props.FromProducer(() => new HelloActor());
            var props2 = Props.FromProducer(() => new HelloActor2());
            var pid = context.Spawn(props);
            var pid2 = context.Spawn(props2);
            var result = await context.RequestAsync<Hello>(pid2, new Hello("ProtoActor"), TimeSpan.FromSeconds(5));

            Assert.AreEqual("ok", result.Who);

        }

        [Test]
        public void TestEscalateException()
        {
            var context = new RootContext();
            var props = Props.FromProducer(() => new FaultyActor());
            var pid = context.Spawn(props);

            Assert.DoesNotThrow(() =>
            {
                context.Send(pid, new Hello("ProtoActor"));
            });

        }

    }
}
