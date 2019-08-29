using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class OneForOneStrategy : ISupervisorStrategy
    {
        private ICluster _cluster;

        public OneForOneStrategy(ICluster cluster)
        {
            _cluster = cluster;
        }


        public Task Handle(ISystemMessage message)
        {
            var process = _cluster.Get(message.Who);

            switch (message)
            {
                case Start _:

                    foreach (var child in process.Children)
                    {
                        _cluster.Start(child.Id);
                    }

                    break;
                case Stop _:

                    foreach (var child in process.Children)
                    {
                        _cluster.Stop(child.Id);
                    }

                    break;
                case Failure failure:

                    //refacto :should be synchronized
                    _cluster.Emit(failure.Who, new Restart(failure.Who, failure.Reason));

                    foreach (var child in process.Children)
                    {
                        _cluster.Emit(child.Id, new Restart(failure.Who, failure.Reason));
                    }

                    break;
                case Restart _:

                    foreach (var child in process.Children)
                    {
                        _cluster.Start(child.Id);
                    }

                    break;

            }

            return Task.CompletedTask;
        }
    }
}
