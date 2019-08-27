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

    public class HelloActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            if (msg is Hello r)
            {
                Console.WriteLine($"Hello {r.Who}");
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
        public async Task Testing()
        {
            var context = new RootContext();
            var props = Props.FromProducer(() => new HelloActor());
            var props2 = Props.FromProducer(() => new HelloActor2());
            var pid = context.Spawn(props);
            var pid2 = context.Spawn(props2);
            context.Request(pid2, new Hello("ProtoActor"), pid);

            await Task.Delay(1000);
        }

    }
}
