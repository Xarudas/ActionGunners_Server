using MeatInc.ActionGunnersServer.MatchSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MeatInc.ActionGunnersServer
{
    public class Test : IInitializable
    {
        private MatchFacade _facade;
        public Test(MatchFacade facade)
        {
            _facade = facade;
        }
        public void Initialize()
        {
           
        }
    }
}
