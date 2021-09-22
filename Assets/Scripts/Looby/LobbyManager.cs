using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeatInc.ActionGunnersServer.Connections;
using DarkRift.Server;
using DarkRift;
using MeatInc.ActionGunnersShared;
using MeatInc.ActionGunnersShared.Looby;
using System;
using System.Linq;
using MeatInc.ActionGunnersServer.RoomSystem;

namespace MeatInc.ActionGunnersServer.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public event Action<LobbyConnection> JoinedLobby;
        public event Action<LobbyConnection> LeavedLobby;
        public static LobbyManager Instance { get; private set; }

        public List<LobbyConnection> UsersInLobby { get; } = new List<LobbyConnection>();

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
            ServerManager.Instance.Connected += OnClientConnect;
            ServerManager.Instance.Disconnected += OnClientDisconnect;
        }

        private void OnDestroy()
        {
            ServerManager.Instance.Connected -= OnClientConnect;
            ServerManager.Instance.Disconnected -= OnClientDisconnect;
        }
        private void OnClientDisconnect(ClientConnection clientConnection)
        {
            LeftLobby(clientConnection.Client);
        }

        private void OnClientConnect(ClientConnection clientConnection)
        {
            clientConnection.Client.MessageReceived += OnMessage;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            IClient client = (IClient)sender;
            using (Message message = e.GetMessage())
            {
                switch (message.Tag)
                {
                    case Tags.Looby.JoinRequest:
                        OnJoinLobby(client);
                        break;
                }
            }
        }

        private void OnJoinLobby(IClient client)
        {
            var userInLooby = UsersInLobby.FirstOrDefault(u => u.Client == client);
            if (userInLooby == null)
            {
                userInLooby = new LobbyConnection(client, new ClientInfoData(client.ID, GenerateName(client.ID)));
                UsersInLobby.Add(userInLooby);
                var lobbyInfoData = new LobbyInfoData(userInLooby.InfoData, RoomManager.Instance.GetRoomsData());
                using (Message message = Message.Create(Tags.Looby.JoinAccepted, lobbyInfoData))
                {
                    client.SendMessage(message, SendMode.Reliable);
                }
                JoinedLobby?.Invoke(userInLooby);
            }
            else
            {
                using (Message message = Message.CreateEmpty(Tags.Looby.JoinDenied))
                {
                    client.SendMessage(message, SendMode.Reliable);
                }
                return;
            }
        }
        public void LeftLobby(IClient client)
        {
            var userInLooby = UsersInLobby.First(u => u.Client == client);
            if (userInLooby != null)
            {
                UsersInLobby.Remove(userInLooby);
                client.MessageReceived -= OnMessage;
            }
        }
        private string GenerateName(ushort clientId)
        {
            return "test_" + clientId;
        }
    }
}
