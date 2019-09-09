using Orleans.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics
{
    public abstract class AggregateBase : IAgreggate
    {
        private readonly List<IEvent> _events;

        protected AggregateBase()
        {
            _events = new List<IEvent>();
        }

        public void Apply(IEvent @event)
        {
            HandleEvent(@event);

            _events.Add(@event);
        }

        protected abstract void HandleEvent(IEvent @event);

        public Task<IEnumerable<IEvent>> GetAppliedEvents()
        {
            return Task.FromResult(_events.AsEnumerable());
        }
    }
}
