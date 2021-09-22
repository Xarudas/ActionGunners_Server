using DarkRift.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.Connections
{
    public class ClientConnection : AbstractClientConnection
    {
        public ClientConnection(IClient client) : base(client) { }
    }
}
