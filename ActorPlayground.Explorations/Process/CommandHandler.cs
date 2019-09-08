using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActorPlayground.Explorations
{
    public class CommandHandler : ICandSend
    {
        private readonly IActorProcess _actorProcess;
        private readonly IActorRegistry _registry;
        private readonly Dictionary<Guid, TaskCompletionSource<ICommandResult>> _pendingCommands;

        public CommandHandler(IActorProcess actorProcess, IActorRegistry registry)
        {
            _actorProcess = actorProcess;
            _registry = registry;
            _pendingCommands = new Dictionary<Guid, TaskCompletionSource<ICommandResult>>();
        }

        private void EnsureCanSend(ICanPost target)
        {
            if (target.Id.Type == ActorType.Remote && _actorProcess.Id.Type == ActorType.Transient)
                throw new Exception("cannot send from transient to remote");
        }

        public void HandleCommandResult(ICommandResult commandResult)
        {
            if (_pendingCommands.ContainsKey(commandResult.CommandId))
            {
                _pendingCommands[commandResult.CommandId].SetResult(commandResult);
                _pendingCommands.Remove(commandResult.CommandId);
            }
        }

        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message, TimeSpan timeout) where TCommandResult : ICommandResult
        {
            var targetProcess = _registry.Get(targetId);

            EnsureCanSend(targetProcess);

            var taskSource = new TaskCompletionSource<ICommandResult>();

            message.CommandId = Guid.NewGuid();

            _pendingCommands.Add(message.CommandId, taskSource);

            targetProcess.Post(message, _actorProcess);

            var cancel = new CancellationTokenSource(timeout);

            cancel.Token.Register(() => taskSource.TrySetCanceled(), false);

            return taskSource.Task.ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully) return (TCommandResult)task.Result;
                if (task.IsCanceled) throw new Exception("timeout");

                throw task.Exception;

            }, cancel.Token);

        }

        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message, CancellationToken cancellationToken) where TCommandResult : ICommandResult
        {
            var targetProcess = _registry.Get(targetId);

            EnsureCanSend(targetProcess);

            var taskSource = new TaskCompletionSource<ICommandResult>();

            message.CommandId = Guid.NewGuid();

            _pendingCommands.Add(message.CommandId, taskSource);

            targetProcess.Post(message, _actorProcess);

            return taskSource.Task.ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully) return (TCommandResult)task.Result;
                if (task.IsCanceled) throw new Exception("timeout");

                throw task.Exception;

            }, cancellationToken);
        }

        //refacto: default timeout
        public Task<TCommandResult> Send<TCommandResult>(string targetId, ICommand message) where TCommandResult : ICommandResult
        {

            var targetProcess = _registry.Get(targetId);

            EnsureCanSend(targetProcess);

            var taskSource = new TaskCompletionSource<ICommandResult>();

            message.CommandId = Guid.NewGuid();

            _pendingCommands.Add(message.CommandId, taskSource);

            targetProcess.Post(message, _actorProcess);

            return taskSource.Task.ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully) return (TCommandResult)task.Result;
                if (task.IsCanceled) throw new Exception("timeout");

                throw task.Exception;

            });
        }

    }
}
