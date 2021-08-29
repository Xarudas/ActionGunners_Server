using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersShared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MeatInc.ActionGunnersServer
{
    [RequireComponent(typeof(PlayerLogic))]
    public class ServerPlayer : MonoBehaviour
    {
        private ClientConnection _clientConnection;
        private Room room;

        private PlayerStateData _currentPlayerStateData;

        private Buffer<PlayerInputData> inputBuffer = new Buffer<PlayerInputData>(1, 2);

        public PlayerLogic PlayerLogic { get; private set; }
        public uint InputTick { get; private set; }
        public IClient Client { get; private set; }
        public PlayerStateData CureentPlayerStateData => _currentPlayerStateData;
        private PlayerInputData[] inputs;

        private void Awake()
        {
            PlayerLogic = GetComponent<PlayerLogic>();
        }

        public void Initialize(Vector3 position, ClientConnection clientConnection)
        {
            _clientConnection = clientConnection;
            room = _clientConnection.Room;
            Client = _clientConnection.Client;
            _clientConnection.Player = this;

            _currentPlayerStateData = new PlayerStateData(Client.ID, 0, position, Quaternion.identity);
            InputTick = room.ServerTick;

            var playerSpawnData = room.GetSpawnDataForAllPlayers();
            using (Message message = Message.Create(Tags.Game.GameStartDataResponse, new GameStartData(playerSpawnData, room.ServerTick)))
            {
                Client.SendMessage(message, SendMode.Reliable);
            }
        }

        public void RecieveInput(PlayerInputData input)
        {
            inputBuffer.Add(input);
        }
        public void PlayerPreUpdate()
        {
            inputs = inputBuffer.Get();
            for (int i = 0; i < inputs.Length; i++)
            {
                /*if (inputs[i].Keyinputs[5])
                {
                    room.PerformShootRayCast(inputs[i].Time, this);
                    return;
                }*/
            }
        }
        public PlayerStateData PlayerUpdate()
        {
            if (inputs.Length > 0)
            {
                PlayerInputData input = inputs.First();
                InputTick++;

                for (int i = 1; i < inputs.Length; i++)
                {
                    InputTick++;
                    for (int j = 0; j < input.KeyInputs.Length; j++)
                    {
                        input.KeyInputs[j] = input.KeyInputs[j] || inputs[i].KeyInputs[j];
                    }
                    input.LookDirection = inputs[i].LookDirection;
                }
                _currentPlayerStateData = PlayerLogic.GetNextFrameData(input, _currentPlayerStateData);
            }

            transform.localPosition = CureentPlayerStateData.Position;
            transform.localRotation = CureentPlayerStateData.LookDirection;

            return CureentPlayerStateData;
        }

        public PlayerSpawnData GetPlayerSpawnData()
        {
            return new PlayerSpawnData(Client.ID, _clientConnection.Login, transform.localPosition);
        }
    }
}

