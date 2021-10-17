using MeatInc.ActionGunnersServer.MatchSystem.Components;
using MeatInc.ActionGunnersShared.Interfaces;
using System;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.MatchSystem.Installers
{
    public class MatchInstaller : MonoInstaller
    {
        [SerializeField]
        private MatchFacade _matchFacade;
        public override void InstallBindings()
        {
            Container.Bind(typeof(MatchFacade), typeof(IEntity), typeof(IDisposable)).FromInstance(_matchFacade).AsSingle();
            Container.BindInterfacesAndSelfTo<JoinOnMatchManagment>().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchClientConnections>().AsSingle();
        }
    }
}