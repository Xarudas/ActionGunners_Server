using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersSharedLegacy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServerLegacy
{
    public class ClientConnection 
    {
        public string Login { get; }
        public IClient Client { get; }
        public Room Room { get; set; }
        public ServerPlayer Player { get; set; }

        public ClientConnection(IClient client, LoginRequestData data)
        {
            Client = client;
            Login = data.Login;

            ServerManager.Instance.Players.Add(client.ID, this);
            ServerManager.Instance.PlayersByName.Add(Login, this);
            

            using (Message message = Message.Create(Tags.Login.LoginRequestAccepted, new LoginInfoData(client.ID, new LobbyInfoData(RoomManager.Instance.GetRoomDataList()))))
            {
                client.SendMessage(message, SendMode.Reliable);
            }

            Client.MessageReceived += OnMessage;
        }

        public void OnClientDiconnect(object sender, ClientDisconnectedEventArgs e)
        {
            ServerManager.Instance.Players.Remove(Client.ID);
            ServerManager.Instance.PlayersByName.Remove(Login);
            if (Room != null)
            {
                Room.RemovePlayerFromRoom(this);
            }
            e.Client.MessageReceived -= OnMessage;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            IClient client = (IClient)sender;

            using (Message message = e.GetMessage())
            {
                switch (message.Tag)
                {
                    case Tags.Lobby.LobbyJoinRoomRequest:
                        RoomManager.Instance.TryJoinRoom(client, message.Deserialize<JoinRoomRequest>());
                        break;
                    case Tags.Game.GameJoinRequest:
                        Room.JoinPlayerToGame(this);
                        break;
                    case Tags.Game.GamePlayerInput:
                        Player.RecieveInput(message.Deserialize<PlayerInputData>());
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
