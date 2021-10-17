using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MeatInc.ActionGunnersServer.MatchSystem
{
    public struct MatchSpawnParameters 
    {
        public ushort Id { get; }

        public MatchSpawnParameters(ushort id)
        {
            Id = id;
        }
    }
}
