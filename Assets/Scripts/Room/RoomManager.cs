using MeatInc.ActionGunnersShared.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeatInc.ActionGunnersServer.Lobby;
using MeatInc.ActionGunnersServer.Connections;
using System;
using DarkRift.Server;
using DarkRift;
using MeatInc.ActionGunnersShared;
using System.Linq;

namespace MeatInc.ActionGunnersServer.RoomSystem
{
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }
        [SerializeField]
        private GameObject _roomPrefab;
        private Dictionary<ushort, Room> _rooms { get; } = new Dictionary<ushort, Room>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this);
        }
        private void Start()
        {
            LobbyManager.Instance.JoinedLobby += OnJoinJobby;
            LobbyManager.Instance.LeavedLobby += OnLeftLobby;
            CreateRoom(new CreateRoomRequest("Test1", 10));
            CreateRoom(new CreateRoomRequest("Test2", 30));
        }
        private void OnDestroy()
        {
            LobbyManager.Instance.JoinedLobby -= OnJoinJobby;
            LobbyManager.Instance.LeavedLobby -= OnLeftLobby;
        }

        private void OnJoinJobby(LobbyConnection connection)
        {
            connection.Client.MessageReceived += OnMessage;
        }
        private void OnLeftLobby(LobbyConnection connection)
        {
            connection.Client.MessageReceived -= OnMessage;
            foreach (var room in _rooms)
            {
                foreach (var roomConnection in room.Value.Connections)
                {
                    if (roomConnection.Client == connection.Client)
                    {
                        room.Value.RemovePlayerFromRoom(roomConnection);
                        return;
                    }
                }
            }
        }
        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            IClient client = (IClient)sender;
            using (Message message = e.GetMessage())
            {
                switch (message.Tag)
                {
                    case Tags.Room.RefreshRequest:
                        OnRefreshRooms(client);
                        break;
                    case Tags.Room.CreateRequest:
                        CreateRoom(client, message.Deserialize<CreateRoomRequest>());
                        break;
                    case Tags.Room.JoinRequest:
                        TryJoinRoom(client, message.Deserialize<JoinRoomRequest>());
                        break;
                    default:
                        break;
                }
            }
        }
        private void OnRefreshRooms(IClient client)
        {
            using (Message message = Message.Create(Tags.Room.RefreshResponse, GetRoomsData()))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
        }
        private void TryJoinRoom(IClient client, JoinRoomRequest data)
        {
            var user = LobbyManager.Instance.UsersInLobby.FirstOrDefault(u => u.InfoData.Id == client.ID);
            bool canJoin = user == null ? false : true;
            if (!_rooms.TryGetValue(data.Id, out var room))
            {
                canJoin = false;
            }
            if (room.Connections.Count >= room.Info.MaxSlots)
            {
                canJoin = false;
            }
            if (canJoin == true)
            {
                room.AddPlayerToRoom(user);
            }
            else
            {
                using (Message message = Message.Create(Tags.Room.JoinDenied, GetRoomsData()))
                {
                    client.SendMessage(message, SendMode.Reliable);
                }
            }
        }
        private RoomData CreateRoom(IClient client, CreateRoomRequest data)
        {
            var room = CreateRoom(data);
            using (Message message = Message.Create(Tags.Room.CreateAccept, room))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
            return room;
        }
        private RoomData CreateRoom(CreateRoomRequest data)
        {
            RoomData roomData = new RoomData(GenerateRoomId(), data.Name, data.MaxSlots);
            GameObject go = Instantiate(_roomPrefab);
            Room room = go.GetComponent<Room>();
            room.Inititalize(roomData);
            _rooms.Add(roomData.Id, room);
            return roomData;
        }
        private void RemoveRoom(ushort id)
        {
            Room room = _rooms[id];
            room.Close();
            _rooms.Remove(id);
        }
        private ushort GenerateRoomId()
        {
            ushort i = 0;
            while (true)
            {
                if (!_rooms.ContainsKey(i))
                {
                    return i;
                }
                i++;
            }
        }

        public RoomsInfoData GetRoomsData()
        {
            RoomsInfoData data = new RoomsInfoData(_rooms.Select(r => r.Value.Info).ToArray());
            return data;
        }
    }
}
