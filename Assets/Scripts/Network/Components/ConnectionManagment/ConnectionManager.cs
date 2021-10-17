using DarkRift.Server;
using MeatInc.ActionGunnersServer.Interfaces.Network.Components.ServerManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.Network.Components.ConnectionManagment
{
    public class ConnectionManager : IInitializable, IDisposable
    {
        public event Action<ClientConnection> ClientConnected;
        public event Action<ClientConnection> ClientDisconnected;
        private List<ClientConnection> Connections { get; } = new List<ClientConnection>();
        private readonly IServerInfo _serverInfo;
        public ConnectionManager(IServerInfo serverInfo)
        {
            _serverInfo = serverInfo;
        }
        public void Initialize()
        {
            _serverInfo.Server.ClientManager.ClientConnected += OnClientConnect;
            _serverInfo.Server.ClientManager.ClientDisconnected += OnClientDisconnect;
        }
        public void Dispose()
        {
            _serverInfo.Server.ClientManager.ClientConnected -= OnClientConnect;
            _serverInfo.Server.ClientManager.ClientDisconnected -= OnClientDisconnect;
        }
        private void OnClientConnect(object sender, ClientConnectedEventArgs e)
        {
            var connection = new ClientConnection(e.Client);
            Connections.Add(connection);
            ClientConnected?.Invoke(connection);
        }
        private void OnClientDisconnect(object sender, ClientDisconnectedEventArgs e)
        {
            var clientConnection = Connections.FirstOrDefault(c => c.Client == e.Client);
            ClientDisconnected?.Invoke(clientConnection);
            if (clientConnection != null)
            {
                Connections.Remove(clientConnection);
            }
        }
        public ClientConnection GetById(ushort id)
        {
            var connection = Connections.FirstOrDefault(c => c.Client.ID == id);
            return connection;
        }
    }
}
