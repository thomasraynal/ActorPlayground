using ActorPlayground.POC.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.POC
{
    public class Cluster : ICluster
    {
        private IActorRegistry _registry;
        private ISupervisor _supervisor;
        private ActorProcess _process;

        public ActorProcess Spawn(IActor actor)
        {
            return _registry.Add(actor, _process);
        }

        public Cluster()
        {
    
        }

        public void Initialize(ISupervisor supervisor, IActorRegistry registry)
        {
            _registry = registry;
            _supervisor = supervisor;
            _process = _registry.Add(this, null);
        }

        public void Emit(string target, object message)
        {
            var process = _registry.Get(target);
            process.Post(message, _process);
        }

        public Task<T> Send<T>(string target, object message, TimeSpan timeout)
        {
            return SendInternal(target, message, new Future<T>(timeout));
        }

        public Task<T> Send<T>(string target, object message, CancellationToken cancellationToken)
        {
            return SendInternal(target, message, new Future<T>(cancellationToken));
        }

        public Task<T> Send<T>(string target, object message)
        {
            return SendInternal(target, message, new Future<T>());
        }

        private Task<T> SendInternal<T>(string target, object message, Future<T> future)
        {
            var targetProcess = _registry.Get(target);
            var futureProcess = _registry.Add(future, _process);

            targetProcess.Post(message, futureProcess);

            return future.UnderlyingTask;
        }

        public ActorProcess Get(string id)
        {
            return _registry.Get(id);
        }

        public void Stop(string id)
        {
            var process = Get(id);

            //shoudl go through Cluster
            Emit(_supervisor.)

            process.Post(new Stop(id), process);
        }

        public void Start(string id)
        {
            var process = Get(id);
            process.Post(new Start(id), process);
        }

        public void Remove(string id)
        {
            Stop(id);

            _registry.Remove(id);
        }

        public Task Receive(IContext context)
        {
            throw new NotImplementedException();
        }

  
    }
}
