using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersServer.RoomSystem;
using MeatInc.ActionGunnersShared;

namespace MeatInc.ActionGunnersServer.Connections
{
    public class RoomConnection : LobbyConnection
    {
        private IRoomInConnection _room;
        public RoomConnection(IClient client, ClientInfoData _clientData, IRoomInConnection room) : base(client, _clientData)
        {
            _room = room;
            client.MessageReceived += OnMessage;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            IClient client = (IClient)sender;

            using (Message message = e.GetMessage())
            {
                switch (message.Tag)
                {
                    case Tags.Game.JoinRequest:
                        _room.JoinPlayerToGame(this);
                        break;
                    case Tags.Room.LeaveRequest:
                        _room.LeftRoom(this);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
