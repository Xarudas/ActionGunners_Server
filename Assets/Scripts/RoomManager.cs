using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServer
{
    public class RoomManager : MonoBehaviour
    {
        Dictionary<string, Room> rooms = new Dictionary<string, Room>();

        public static RoomManager Instance;

        [SerializeField]
        private GameObject _roomPrefab;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
            CreateRoom("Main", 25);
            CreateRoom("Main 2", 15);
        }

        public void CreateRoom(string roomName, byte maxSlots)
        {
            GameObject go = Instantiate(_roomPrefab);
            Room room = go.GetComponent<Room>();
            room.Inititalize(roomName, maxSlots);
            rooms.Add(roomName, room);
        }

        public void RemoveRoom(string roomName)
        {
            Room room = rooms[roomName];
            room.Close();
            rooms.Remove(name);
        }

        public RoomData[] GetRoomDataList()
        {
            RoomData[] data = new RoomData[rooms.Count];
            int i = 0;
            foreach (var room in rooms)
            {
                Room r = room.Value;
                data[i] = new RoomData(r.Name, (byte)r.ClientConnections.Count, r.MaxSlots);
                i++;
            }
            return data;
        }
        
        public void TryJoinRoom(IClient client, JoinRoomRequest data)
        {
            bool canJoin = ServerManager.Instance.Players.TryGetValue(client.ID, out var clientConnection);

            if (!rooms.TryGetValue(data.RoomName, out var room))
            {
                canJoin = false;
            }
            else if (room.ClientConnections.Count >= room.MaxSlots)
            {
                canJoin = false;
            }

            if (canJoin)
            {
                room.AddPlayerToRoom(clientConnection);
            }
            else
            {
                using (Message message = Message.Create(Tags.Lobby.LobbyJoinRoomDenied, new LobbyInfoData(GetRoomDataList())))
                {
                    client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
    }
}
