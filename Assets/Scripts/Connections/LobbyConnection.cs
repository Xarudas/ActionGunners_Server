using DarkRift.Server;
using MeatInc.ActionGunnersShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeatInc.ActionGunnersServer.Connections
{
    public class LobbyConnection : ClientConnection
    {
        public ClientInfoData InfoData;
        public LobbyConnection(IClient client, ClientInfoData data) : base(client)
        {
            InfoData = data;
        }
    }
}
