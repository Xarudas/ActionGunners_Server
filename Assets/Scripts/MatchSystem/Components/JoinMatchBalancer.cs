using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersServer.Network.Components.ConnectionManagment;
using MeatInc.ActionGunnersShared;
using MeatInc.ActionGunnersShared.Interfaces.Containers;
using MeatInc.ActionGunnersShared.MatchSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.MatchSystem.Components
{
    public class JoinMatchBalancer : IInitializable, IDisposable
    {
        private readonly ConnectionManager _connectionManager;
        private readonly IEntityComponentContainer<JoinMatchParam> _joinManagmentContainer;

        public JoinMatchBalancer(ConnectionManager connectionManager, IEntityComponentContainer<JoinMatchParam> joinManagmentContainer)
        {
            _connectionManager = connectionManager;
            _joinManagmentContainer = joinManagmentContainer;
        }
        public void Initialize()
        {
            _connectionManager.ClientConnected += OnClientConnect;
            _connectionManager.ClientDisconnected += OnClientDisconnect;
        }
        public void Dispose()
        {
            _connectionManager.ClientConnected -= OnClientConnect;
            _connectionManager.ClientDisconnected -= OnClientDisconnect;
        }
        private void OnClientConnect(ClientConnection connection)
        {
            connection.Client.MessageReceived += JoinToMatch;
        }
        private void OnClientDisconnect(ClientConnection connection)
        {
            connection.Client.MessageReceived -= JoinToMatch;
        }

        private void JoinToMatch(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            {
                ushort tag = message.Tag;
                if (tag == Tags.JoinMatch)
                {
                    var client = (IClient)sender;
                    var data = message.Deserialize<JoinMatchData>();
                    foreach (var component in _joinManagmentContainer.Components)
                    {
                        if (component.Entity.Id == data.MatchId)
                        {
                            var clientConnection = _connectionManager.GetById(client.ID);
                            var param = new JoinMatchParam(clientConnection);
                            component.Handle(param);
                        }
                    }
                }
            }
        }
    }
}
