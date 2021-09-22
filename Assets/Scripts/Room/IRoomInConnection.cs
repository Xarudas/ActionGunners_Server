using DarkRift.Server;
using MeatInc.ActionGunnersServer.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeatInc.ActionGunnersServer.RoomSystem
{
    public interface IRoomInConnection
    {
        void LeftRoom(RoomConnection connection);
        void JoinPlayerToGame(RoomConnection connection);
    }
}
