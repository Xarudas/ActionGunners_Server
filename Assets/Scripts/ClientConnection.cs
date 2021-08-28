using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServer
{
    public class ClientConnection 
    {
        public string Login { get; }
        public IClient Client { get; }

        public ClientConnection(IClient client, LoginRequestData data)
        {
            Client = client;
            Login = data.Login;

            ServerManager.Instance.Players.Add(client.ID, this);
            ServerManager.Instance.PlayersByName.Add(Login, this);

            using (Message message = Message.Create(Tags.Login.LoginRequestAccepted, new LoginInfoData(client.ID, new LobbyInfoData())))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
        }
    }
}
