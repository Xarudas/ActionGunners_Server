using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MeatInc.ActionGunnersShared;

namespace MeatInc.ActionGunnersServer
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerPrefab;
        private Scene scene;
        private PhysicsScene _physicsScene;
        private List<ServerPlayer> _serverPlayers = new List<ServerPlayer>();
        private List<PlayerStateData> _playerStateDatas = new List<PlayerStateData>(4);
        private List<PlayerSpawnData> _playerSpawnDatas = new List<PlayerSpawnData>(4);
        private List<PlayerDespawnData> _playerDespawnDatas = new List<PlayerDespawnData>(4);

        public uint ServerTick;
        public string Name;
        public List<ClientConnection> ClientConnections = new List<ClientConnection>();
        public byte MaxSlots;


        private void FixedUpdate()
        {
            ServerTick++;

            foreach (var player in _serverPlayers)
            {
                player.PlayerPreUpdate();
            }
            for (int i = 0; i < _serverPlayers.Count; i++)
            {
                ServerPlayer player = _serverPlayers[i];
                _playerStateDatas[i] = player.PlayerUpdate();
            }

            PlayerStateData[] playerStateDatas = _playerStateDatas.ToArray();
            PlayerSpawnData[] playerSpawnDatas = _playerSpawnDatas.ToArray();
            PlayerDespawnData[] playerDespawnDatas = _playerDespawnDatas.ToArray();

            foreach (var player in _serverPlayers)
            {
                using (Message message = Message.Create(Tags.Game.GameUpdate, new GameUpdateData(player.InputTick, playerStateDatas, playerSpawnDatas, playerDespawnDatas)))
                {
                    player.Client.SendMessage(message, SendMode.Reliable);
                }
            }

            _playerSpawnDatas.Clear();
            _playerDespawnDatas.Clear();
        }
        public void Inititalize(string name, byte maxSlots)
        {
            Name = name;
            MaxSlots = maxSlots;
            CreateSceneParameters csp = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            scene = SceneManager.CreateScene("Room_" + name, csp);
            _physicsScene = scene.GetPhysicsScene();

            SceneManager.MoveGameObjectToScene(gameObject, scene);
        }

        public void AddPlayerToRoom(ClientConnection clientConnection)
        {
            ClientConnections.Add(clientConnection);
            clientConnection.Room = this;
            using (Message message = Message.CreateEmpty(Tags.Lobby.LobbyJoinRoomAccepted))
            {
                clientConnection.Client.SendMessage(message, SendMode.Reliable);
            }
        }

        public void RemovePlayerFromRoom(ClientConnection clientConnection)
        {
            Destroy(clientConnection.Player.gameObject);
            _playerDespawnDatas.Add(new PlayerDespawnData(clientConnection.Client.ID));
            ClientConnections.Remove(clientConnection);
            _serverPlayers.Remove(clientConnection.Player);
            clientConnection.Room = null;
        }

        public void Close()
        {
            foreach (var player in ClientConnections)
            {
                RemovePlayerFromRoom(player);
            }
            Destroy(gameObject);
        }

        public PlayerSpawnData[] GetSpawnDataForAllPlayers()
        {
            PlayerSpawnData[] playerSpawnData = new PlayerSpawnData[_serverPlayers.Count];
            for (int i = 0; i < _serverPlayers.Count; i++)
            {
                ServerPlayer p = _serverPlayers[i];
                playerSpawnData[i] = p.GetPlayerSpawnData();
            }

            return playerSpawnData;
        }

        public void JoinPlayerToGame(ClientConnection clientConnection)
        {
            GameObject go = Instantiate(_playerPrefab, transform);
            ServerPlayer player = go.GetComponent<ServerPlayer>();
            _serverPlayers.Add(player);
            _playerStateDatas.Add(default);
            player.Initialize(Vector3.zero, clientConnection);

            _playerSpawnDatas.Add(player.GetPlayerSpawnData());
        }
    }
}
