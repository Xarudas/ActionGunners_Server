using MeatInc.ActionGunnersServer.Network.Components.ConnectionManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.MatchSystem.Components
{
    public class MatchClientConnections : IInitializable, IDisposable
    {
        public Action<ClientConnection> ClientDisconnected;
        public IReadOnlyList<ClientConnection> Connections => _connections;

        private readonly List<ClientConnection> _connections;
        private readonly ConnectionManager _connectionManager;

        public MatchClientConnections(ConnectionManager connectionManager)
        {
            _connections = new List<ClientConnection>();
            _connectionManager = connectionManager;
        }

        public void AddConnection(ClientConnection clientConnection)
        {
            _connections.Add(clientConnection);
        }

        public void Initialize()
        {
            _connectionManager.ClientDisconnected += ClientDiconnect;
        }
        public void Dispose()
        {
            _connectionManager.ClientDisconnected -= ClientDiconnect;
        }
        private void ClientDiconnect(ClientConnection clientConnection)
        {
            var connection = _connections.SingleOrDefault(c => c == clientConnection);
            if (connection != null)
            {
                ClientDisconnected?.Invoke(connection);
                _connections.Remove(connection);
            }
        }

        
    }
}
