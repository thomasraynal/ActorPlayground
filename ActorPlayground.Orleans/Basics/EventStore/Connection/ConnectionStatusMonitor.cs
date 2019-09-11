using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;

namespace ActorPlayground.Orleans.Basics.EventStore
{

    public class ConnectionStatusMonitor : IConnectionStatusMonitor
    {
        private readonly IConnectableObservable<ConnectionInfo> _connectionInfoChanged;
        private readonly IDisposable _connection;
        private readonly IEventStoreConnection _eventStoreConnection;
        private BehaviorSubject<bool> _isConnected;

        public IObservable<bool> IsConnected
        {
            get
            {
                return _isConnected.AsObservable();
            }
        }

        public ConnectionStatusMonitor(IEventStoreConnection connection)
        {

            _isConnected = new BehaviorSubject<bool>(false);

            connection.ConnectAsync().Wait();

            var connected = Observable.FromEventPattern<ClientConnectionEventArgs>(h => connection.Connected += h, h => connection.Connected -= h).Select(_ =>
            {
                return ConnectionStatus.Connected;
            });

            var disconnected = Observable.FromEventPattern<ClientConnectionEventArgs>(h => connection.Disconnected += h, h => connection.Disconnected -= h).Select(_ =>
            {
                return ConnectionStatus.Disconnected;
            });

            var reconnecting = Observable.FromEventPattern<ClientReconnectingEventArgs>(h => connection.Reconnecting += h, h => connection.Reconnecting -= h).Select(_ =>
            {
                return ConnectionStatus.Connecting;
            });

            var closed = Observable.FromEventPattern<ClientClosedEventArgs>(h => connection.Closed += h, h => connection.Closed -= h).Select(arg =>
            {
                return ConnectionStatus.Closed;
            });

            var errorOccurred = Observable.FromEventPattern<ClientErrorEventArgs>(h => connection.ErrorOccurred += h, h => connection.ErrorOccurred -= h).Select(arg =>
            {
                return ConnectionStatus.ErrorOccurred;
            });

            var authenticationFailed = Observable.FromEventPattern<ClientAuthenticationFailedEventArgs>(h => connection.AuthenticationFailed+= h, h => connection.AuthenticationFailed -= h).Select(arg =>
            {
                return ConnectionStatus.AuthenticationFailed;
            });

            _connectionInfoChanged = Observable.Merge(connected, disconnected, reconnecting, closed, errorOccurred, authenticationFailed)
                                               .Scan(ConnectionInfo.Initial, UpdateConnectionInfo)
                                               .StartWith(ConnectionInfo.Initial)
                                               .Do(c => _isConnected.OnNext(c.Status ==  ConnectionStatus.Connected))
                                               .Replay(1);

            _connection = _connectionInfoChanged.Connect();

            _eventStoreConnection = connection;

        }


        public void Dispose()
        {
            _connection.Dispose();
        }

        public IObservable<IConnected<IEventStoreConnection>> GetEventStoreConnectedStream()
        {
            return _connectionInfoChanged
                          .Where(con => con.Status == ConnectionStatus.Connected || con.Status == ConnectionStatus.Disconnected)
                          .Select(con => con.Status == ConnectionStatus.Connected ? Connected.Yes(_eventStoreConnection) : Connected.No<IEventStoreConnection>());
        }

        private ConnectionInfo UpdateConnectionInfo(ConnectionInfo previousConnectionInfo, ConnectionStatus connectionStatus)
        {
            ConnectionInfo newConnectionInfo;

            if ((previousConnectionInfo.Status == ConnectionStatus.Disconnected || previousConnectionInfo.Status == ConnectionStatus.Connecting) && connectionStatus == ConnectionStatus.Connected)
            {
                newConnectionInfo = new ConnectionInfo(connectionStatus, previousConnectionInfo.ConnectCount + 1);
            }
            else
            {
                newConnectionInfo = new ConnectionInfo(connectionStatus, previousConnectionInfo.ConnectCount);
            }

            return newConnectionInfo;
        }
    }
}