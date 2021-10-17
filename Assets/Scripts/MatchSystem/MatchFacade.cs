using MeatInc.ActionGunnersShared.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace MeatInc.ActionGunnersServer.MatchSystem
{
    public class MatchFacade : MonoBehaviour, IEntity, IDisposable
    {
        public event Action Closed;
        public ushort Id => _id;
        public Scene Scene { get; private set; }
        public PhysicsScene PhysicsScene { get; private set; }

        [SerializeField]
        private ushort _id;

        public void OnSpawn(MatchSpawnParameters param, Scene scene)
        {
            _id = param.Id;
            Scene = scene;
            PhysicsScene = Scene.GetPhysicsScene();
        }

        public void Close()
        {
            SceneManager.UnloadSceneAsync(Scene);
        }
        public void Dispose()
        {
            Close();
        }

        public class Factory : PlaceholderFactory<MatchSpawnParameters, MatchFacade>
        {

        }
    }

}
