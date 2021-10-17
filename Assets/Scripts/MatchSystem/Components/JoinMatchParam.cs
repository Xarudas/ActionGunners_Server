using MeatInc.ActionGunnersServer.Network.Components.ConnectionManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.MatchSystem.Components
{
    public struct JoinMatchParam
    {
        public ClientConnection ClientConnection { get; private set; }

        public JoinMatchParam(ClientConnection clientConnection)
        {
            ClientConnection = clientConnection;
        }
    }
}
