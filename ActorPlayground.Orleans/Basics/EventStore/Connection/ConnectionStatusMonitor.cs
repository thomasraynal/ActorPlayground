using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ActorPlayground.Orleans.Basics.EventStore
{

    public class ConnectionStatusMonitor : IConnectionStatusMonitor
    {
        private readonly IConnectableObservable<ConnectionInfo> _connectionInfoChanged;
        private readonly IDisposable _cleanup;
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly BehaviorSubject<bool> _isConnected;

        public bool IsConnected
        {
            get
            {
                return _isConnected.Value;
            }
        }

        public async Task Connect()
        {
           await _eventStoreConnection.ConnectAsync();
        }

        public ConnectionStatusMonitor(IEventStoreConnection eventStoreConnection)
        {
            _eventStoreConnection = eventStoreConnection;
            _isConnected = new BehaviorSubject<bool>(false);


            var connected = Observable.FromEventPattern<ClientConnectionEventArgs>(h => _eventStoreConnection.Connected += h, h => _eventStoreConnection.Connected -= h).Select(_ =>
            {
                return ConnectionStatus.Connected;
            });

            var disconnected = Observable.FromEventPattern<ClientConnectionEventArgs>(h => _eventStoreConnection.Disconnected += h, h => _eventStoreConnection.Disconnected -= h).Select(_ =>
            {
                return ConnectionStatus.Disconnected;
            });

            var reconnecting = Observable.FromEventPattern<ClientReconnectingEventArgs>(h => _eventStoreConnection.Reconnecting += h, h => _eventStoreConnection.Reconnecting -= h).Select(_ =>
            {
                return ConnectionStatus.Connecting;
            });

            var closed = Observable.FromEventPattern<ClientClosedEventArgs>(h => _eventStoreConnection.Closed += h, h => _eventStoreConnection.Closed -= h).Select(arg =>
            {
                return ConnectionStatus.Closed;
            });

            var errorOccurred = Observable.FromEventPattern<ClientErrorEventArgs>(h => _eventStoreConnection.ErrorOccurred += h, h => _eventStoreConnection.ErrorOccurred -= h).Select(arg =>
            {
                return ConnectionStatus.ErrorOccurred;
            });

            var authenticationFailed = Observable.FromEventPattern<ClientAuthenticationFailedEventArgs>(h => _eventStoreConnection.AuthenticationFailed+= h, h => _eventStoreConnection.AuthenticationFailed -= h).Select(arg =>
            {
                return ConnectionStatus.AuthenticationFailed;
            });

            _connectionInfoChanged = Observable.Merge(connected, disconnected, reconnecting, closed, errorOccurred, authenticationFailed)
                                               .Scan(ConnectionInfo.Initial, UpdateConnectionInfo)
                                               .Replay(1);

            _cleanup = _connectionInfoChanged.Connect();

        }

        public void Dispose()
        {
            _cleanup.Dispose();
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

            _isConnected.OnNext(connectionStatus == ConnectionStatus.Connected);

            Debug.WriteLine(connectionStatus);

            return newConnectionInfo;
        }
    }
}