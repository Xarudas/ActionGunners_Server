using MeatInc.ActionGunnersServer.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.Game
{
    public class ServerPlayer : MonoBehaviour
    {
        public RoomConnection Connection { get; private set; }


        public void Initialize(Vector3 position, RoomConnection connection)
        {
            Connection = connection;
        }


    }
}
