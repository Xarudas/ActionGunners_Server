using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer.MatchSystem.Installers
{

    [CreateAssetMenu(fileName = "MatchPoolableInstaller", menuName = "Installers/MatchPoolableInstaller")]
    public class MatchPoolableInstaller : ScriptableObjectInstaller<MatchPoolableInstaller>
    {
        public override void InstallBindings()
        {

        }
    }
}