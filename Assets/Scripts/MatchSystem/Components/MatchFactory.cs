using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace MeatInc.ActionGunnersServer.MatchSystem.Components
{
    public class MatchFactory : IFactory<MatchSpawnParameters, MatchFacade>
    {
        private readonly DiContainer _container;
        private readonly MatchFacade _prefab;

        public MatchFactory(
            DiContainer container, 
            MatchFacade prefab)
        {
            _container = container;
            _prefab = prefab;
        }

        public MatchFacade Create(MatchSpawnParameters param)
        {
            var scene = SceneManager.CreateScene($"Match_{param.Id}", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            var matchFacadeObject = _container.InstantiatePrefab(_prefab);
            SceneManager.MoveGameObjectToScene(matchFacadeObject, scene);
            var matchFacade = matchFacadeObject.GetComponent<MatchFacade>();
            matchFacade.OnSpawn(param, scene);
            var sceneContext = matchFacadeObject.GetComponent<SceneContext>();
            sceneContext.Run();

            return matchFacade;
        }
    }
}
