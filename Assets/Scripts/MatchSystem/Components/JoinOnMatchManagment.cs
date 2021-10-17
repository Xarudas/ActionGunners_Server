using DarkRift;
using MeatInc.ActionGunnersShared;
using MeatInc.ActionGunnersShared.ComponentHandlers;
using MeatInc.ActionGunnersShared.Containers;
using MeatInc.ActionGunnersShared.Interfaces;
using MeatInc.ActionGunnersShared.Interfaces.Containers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.MatchSystem.Components
{
    public class JoinOnMatchManagment : BaseEntityComponentHandler<JoinMatchParam>
    {
        private readonly MatchClientConnections _matchClientConnections;
        public JoinOnMatchManagment(IEntity entity, IEntityComponentContainer<JoinMatchParam> container, MatchClientConnections matchClientConnections) : base(entity, container)
        {
            _matchClientConnections = matchClientConnections;
        }

        public override void Handle(JoinMatchParam param)
        {
            var connection = _matchClientConnections.Connections.SingleOrDefault(c => c == param.ClientConnection);
            if (connection != null)
            {
                using (Message message = Message.CreateEmpty(Tags.JoinMatchDenied))
                {
                    connection.Client.SendMessage(message, SendMode.Reliable);
                    return;
                }
            }
            connection = param.ClientConnection;
            using (Message message = Message.CreateEmpty(Tags.JoimMatchAccepted))
            {
                connection.Client.SendMessage(message, SendMode.Reliable);
                _matchClientConnections.AddConnection(connection);
            }
        }

    }
}
