using DarkRift.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.Interfaces.Network.Components.ServerManagment
{
    public interface IServerInfo
    {
        DarkRiftServer Server { get; }
    }
}
