using DarkRift.Server;
using DarkRift.Server.Unity;
using MeatInc.ActionGunnersServer.Connections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeatInc.ActionGunnersServer
{
    [RequireComponent(typeof(XmlUnityServer))]
    public class ServerManager : MonoBehaviour
    {
        public event Action<ClientConnection> Connected;
        public event Action<ClientConnection> Disconnected;
        public List<ClientConnection> Clients { get; } = new List<ClientConnection>();
        public static ServerManager Instance { get; private set; }

        private XmlUnityServer _xmlServer;
        private DarkRiftServer _server;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            _xmlServer = GetComponent<XmlUnityServer>();
            _server = _xmlServer.Server;
            _server.ClientManager.ClientConnected += OnClientConnected;
            _server.ClientManager.ClientDisconnected += OnClientDiconnected;
        }

        private void OnDestroy()
        {
            _server.ClientManager.ClientConnected -= OnClientConnected;
            _server.ClientManager.ClientDisconnected -= OnClientDiconnected;
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            var clientConnection = new ClientConnection(e.Client);
            Clients.Add(clientConnection);
            Connected?.Invoke(clientConnection);
        }
        private void OnClientDiconnected(object sender, ClientDisconnectedEventArgs e)
        {
            var clientConnection = Clients.FirstOrDefault(c => c.Client == e.Client);
            if (clientConnection != null)
            {
                Disconnected?.Invoke(clientConnection);
                Clients.Remove(clientConnection);
            }
            
        }

        

    }
}
