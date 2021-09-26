using DarkRift.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.Network.Components.ConnectionManagment
{
    public class ClientConnection : IInitializable, IDisposable
    {
        public IClient Client { get; }
        public NetworkRelay _networkRelay { get; }

        public ClientConnection(IClient client)
        {
            Client = client;
            _networkRelay = new NetworkRelay(client);
        }
        public void Initialize()
        {
            _networkRelay.Initialize();
        }

        public void Dispose()
        {
            _networkRelay.Dispose();
        }
    }
}
