using DarkRift;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MeatInc.ActionGunnersSharedLegacy;
using System;

namespace MeatInc.ActionGunnersServerLegacy
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
        private List<PlayerHealthUpdateData> _playerHealthUpdateDatas = new List<PlayerHealthUpdateData>(4);

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
            PlayerHealthUpdateData[] playerHealthUpdateDatas = _playerHealthUpdateDatas.ToArray();

            foreach (var player in _serverPlayers)
            {
                using (Message message = Message.Create(Tags.Game.GameUpdate, new GameUpdateData(player.InputTick, playerStateDatas, playerSpawnDatas, playerDespawnDatas, playerHealthUpdateDatas)))
                {
                    player.Client.SendMessage(message, SendMode.Reliable);
                }
            }

            _playerSpawnDatas.Clear();
            _playerDespawnDatas.Clear();
            _playerHealthUpdateDatas.Clear();
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

        public void UpdatePlayerHealth(ServerPlayer player, byte health)
        {
            _playerHealthUpdateDatas.Add(new PlayerHealthUpdateData(player.Client.ID, health));
        }

        public void PerformShootRayCast(uint frame, ServerPlayer shooter)
        {
            int dif = (int)(ServerTick - 1 - frame);

            Vector3 startPosition;
            Vector3 direction;

            if (shooter.PlayerStateDataHistory.Count > dif)
            {
                startPosition = shooter.PlayerStateDataHistory[dif].Position;
                direction = shooter.PlayerStateDataHistory[dif].LookDirection * Vector3.forward;
            }
            else
            {
                startPosition = shooter.CurentPlayerStateData.Position;
                direction = shooter.CurentPlayerStateData.LookDirection * Vector3.forward;
            }

            startPosition += direction * 3f;

            foreach (var player in _serverPlayers)
            {
                if (player.PlayerStateDataHistory.Count > dif)
                {
                    player.PlayerLogic.CharacterController.enabled = false;
                    player.transform.localPosition = player.PlayerStateDataHistory[dif].Position;
                }
            }

            RaycastHit hit;
            if (_physicsScene.Raycast(startPosition, direction, out hit, 200f))
            {
                if (hit.transform.CompareTag("Unit"))
                {
                    hit.transform.GetComponent<ServerPlayer>().TakeDamage(5);
                }
            }

            foreach (ServerPlayer player in _serverPlayers)
            {
                player.transform.localPosition = player.CurentPlayerStateData.Position;
                player.PlayerLogic.CharacterController.enabled = true;
            }
        }
    }
}
