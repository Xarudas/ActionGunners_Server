using DarkRift.Server;
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
        private List<ClientConnection> Connections { get; } = new List<ClientConnection>();
        private readonly DarkRiftServer _server;
        public ConnectionManager(DarkRiftServer server)
        {
            _server = server;
        }
        public void Initialize()
        {
            _server.ClientManager.ClientConnected += OnClientConnect;
            _server.ClientManager.ClientDisconnected += OnClientDisconnect;
        }
        public void Dispose()
        {
            _server.ClientManager.ClientConnected -= OnClientConnect;
            _server.ClientManager.ClientDisconnected -= OnClientDisconnect;
        }
        private void OnClientConnect(object sender, ClientConnectedEventArgs e)
        {
            Connections.Add(new ClientConnection(e.Client));
        }
        private void OnClientDisconnect(object sender, ClientDisconnectedEventArgs e)
        {
            var clientConnection = Connections.FirstOrDefault(c => c.Client == e.Client);
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
