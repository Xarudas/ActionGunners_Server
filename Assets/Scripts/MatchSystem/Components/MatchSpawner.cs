using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace MeatInc.ActionGunnersServer.MatchSystem.Components
{
    public class MatchSpawner : IInitializable, IDisposable
    {
        private MatchFacade.Factory _factory;

        public MatchSpawner(MatchFacade.Factory factory)
        {
            _factory = factory;
        }

        public void Dispose()
        {

        }

        public void Initialize()
        {
            _factory.Create(new MatchSpawnParameters(0));
        }
    }
}
