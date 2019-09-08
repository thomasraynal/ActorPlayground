using ActorPlayground.Explorations.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public class OneForOneStrategy : ISupervisorStrategy
    {
        public Task HandleFailure(IActorProcess self, Failure failure)
        {
            self.Post(new Restart(self.Configuration.Id.Value, failure.Reason), self);

            foreach(var child in self.Children)
            {
                child.Post(new Restart(child.Configuration.Id.Value, failure.Reason), self);
            }

            return Task.CompletedTask;

        }
    }
}
