using DarkRift.Server.Unity;
using MeatInc.ActionGunnersServer.MatchSystem;
using MeatInc.ActionGunnersServer.MatchSystem.Components;
using MeatInc.ActionGunnersServer.Network.Components.ConnectionManagment;
using MeatInc.ActionGunnersServer.Network.Components.ServerManagment;
using MeatInc.ActionGunnersShared.Containers;
using MeatInc.ActionGunnersShared.Interfaces.Containers;
using System;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.Network.Installers
{
    public class ProjectNetworkInstaller : MonoInstaller
    {
        [SerializeField]
        private ServerManager _serverManager;
        [SerializeField]
        private MatchFacade _matchFacade;
        public override void InstallBindings()
        {
            InstallServer();
            Container.BindInterfacesAndSelfTo<ConnectionManager>().AsSingle();
            InstallMatchSystem();
            
        }
        private void InstallServer()
        {
            Container.BindInterfacesAndSelfTo<ServerInfo>().AsSingle();
            Container.BindInstance(_serverManager).AsSingle();
        }

        private void InstallMatchSystem()
        {
            Container.BindFactory<MatchSpawnParameters, MatchFacade, MatchFacade.Factory>().FromFactory<MatchFactory>();
            Container.BindInstance(_matchFacade).WhenInjectedInto<MatchFactory>();
            Container.BindInterfacesAndSelfTo<MatchSpawner>().AsSingle();
            Container.BindInterfacesAndSelfTo<JoinMatchBalancer>().AsSingle();
            Container.Bind(typeof(IEntityComponentContainer<>)).To(typeof(EntityComponentContainer<>)).AsSingle();
        }
    }
}