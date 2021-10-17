using DarkRift.Server;
using MeatInc.ActionGunnersServer.Interfaces.Network.Components.ServerManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.Network.Components.ServerManagment
{
    public class ServerInfo : IServerInfo
    {
        public DarkRiftServer Server { get; private set; }

        public void ReplaceServer(DarkRiftServer server)
        {
            Server = server;
        }
    }
}
