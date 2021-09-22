using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersServer.Connections;
using MeatInc.ActionGunnersServer.Game;
using MeatInc.ActionGunnersShared;
using MeatInc.ActionGunnersShared.Room;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MeatInc.ActionGunnersServer.RoomSystem
{
    public class Room : MonoBehaviour, IRoomInConnection
    {
        public RoomData Info { get; private set; }
        public IReadOnlyList<RoomConnection> Connections { get => _connections; } 

        private Scene _scene;
        private ServerGameLoop _serverGameLoop;
        private readonly List<RoomConnection> _connections = new List<RoomConnection>();

        public void Inititalize(RoomData info)
        {
            Info = info;
            CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            _scene = SceneManager.CreateScene("Room_" + info.Id, csp);
            _serverGameLoop = GetComponent<ServerGameLoop>();
            _serverGameLoop.Initialize(_scene.GetPhysicsScene());
            DisableMeshsInRoom();

            SceneManager.MoveGameObjectToScene(gameObject, _scene);
        }
        public void AddPlayerToRoom(LobbyConnection user)
        {
            var connection = new RoomConnection(user.Client, user.InfoData, this);
            _connections.Add(connection);
            using (Message message = Message.CreateEmpty(Tags.Room.JoinAccept))
            {
                connection.Client.SendMessage(message, SendMode.Reliable);
            }
        }

        public void RemovePlayerFromRoom(RoomConnection connection)
        {
            _serverGameLoop.RemovePlayerFromLoop(connection);
            _connections.Remove(connection);
        }
        public void Close()
        {
            foreach (var connection in Connections)
            {
                RemovePlayerFromRoom(connection);
            }
            SceneManager.UnloadSceneAsync(_scene);
            Destroy(gameObject);
        }

        public void JoinPlayerToGame(RoomConnection connection)
        {
            bool canJoin = _connections.Contains(connection);
            if (canJoin == true)
            {
                _serverGameLoop.AddPlayerToLoop(connection);
            }
        }
        public void LeftRoom(RoomConnection connection)
        {
            RemovePlayerFromRoom(connection);
            using (Message message = Message.CreateEmpty(Tags.Room.LeaveResponse))
            {
                connection.Client.SendMessage(message, SendMode.Reliable);
            }
        }
        private void DisableMeshsInRoom()
        {
            var meshRenderes = GetComponentsInChildren<MeshRenderer>(true);
            var meshFilters = GetComponentsInChildren<MeshFilter>(true);
            foreach (var mesh in meshRenderes)
            {
                Destroy(mesh);
            }
            foreach (var mesh in meshFilters)
            {
                Destroy(mesh);
            }
        }
    }
}
