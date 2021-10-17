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
        public NetworkRelay NetworkRelay { get; }

        public ClientConnection(IClient client)
        {
            Client = client;
            NetworkRelay = new NetworkRelay(client);
        }
        public void Initialize()
        {
            NetworkRelay.Initialize();
        }

        public void Dispose()
        {
            NetworkRelay.Dispose();
        }
    }
}
