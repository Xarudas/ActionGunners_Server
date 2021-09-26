using DarkRift.Server.Unity;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.Network.Installers
{
    public class ProjectNetworkInstaller : MonoInstaller
    {
        [SerializeField]
        private XmlUnityServer _xmlUnityServer;
        public override void InstallBindings()
        {
            Container.BindInstance(_xmlUnityServer).AsSingle().NonLazy();
            Container.BindInstance(_xmlUnityServer.Server).AsSingle();
        }
    }
}