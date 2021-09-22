using MeatInc.ActionGunnersServer.Connections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.Game
{
    public class ServerGameLoop : MonoBehaviour
    {
        [SerializeField]
        private GameObject _playerPrefab;
        private PhysicsScene _scene;
        private List<ServerPlayer> _serverPlayers { get; } = new List<ServerPlayer>();

        public void Initialize(PhysicsScene scene)
        {
            _scene = scene;
        }


        public void RemovePlayerFromLoop(RoomConnection connection)
        {
            var player = _serverPlayers.FirstOrDefault(p => p.Connection == connection);
            if (player != null)
            {
                Destroy(player.gameObject);
                //_playerDespawnDatas.Add(new PlayerDespawnData(clientConnection.Client.ID));
                _serverPlayers.Remove(player);
            }
        }

        public void AddPlayerToLoop(RoomConnection connection)
        {
            GameObject go = Instantiate(_playerPrefab, transform);
            ServerPlayer player = go.GetComponent<ServerPlayer>();
            _serverPlayers.Add(player);
        }
    }

}
