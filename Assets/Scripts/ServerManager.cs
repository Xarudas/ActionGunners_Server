using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using MeatInc.ActionGunnersShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServer
{
    public class ServerManager : MonoBehaviour
    {
        public static ServerManager Instance;
        public Dictionary<ushort, ClientConnection> Players = new Dictionary<ushort, ClientConnection>();
        public Dictionary<string, ClientConnection> PlayersByName = new Dictionary<string, ClientConnection>();

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
            e.Client.MessageReceived += OnMessage;
        }

        private void OnClientDiconnected(object sender, ClientDisconnectedEventArgs e)
        {
            e.Client.MessageReceived -= OnMessage;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            IClient client = (IClient)sender;
            using (Message message = e.GetMessage())
            {
                switch (e.Tag)
                {
                    case Tags.Login.LoginRequest:
                        OnClientLogin(client, message.Deserialize<LoginRequestData>());
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnClientLogin(IClient client, LoginRequestData loginRequestData)
        {
            if (PlayersByName.ContainsKey(loginRequestData.Login))
            {
                using (Message message = Message.CreateEmpty(Tags.Login.LoginRequestDenied))
                {
                    client.SendMessage(message, SendMode.Reliable);
                }
                return;
            }

            client.MessageReceived -= OnMessage;

            new ClientConnection(client, loginRequestData);
        }
    }

}
