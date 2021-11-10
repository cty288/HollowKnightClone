using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public abstract class AbstractSystem : ISystem
    {
        private IArchitecture architectureModel;
        
        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return architectureModel;
        }

      void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architectureModel = architecture;
        }

        void ISystem.Init()
        {
            OnInit();
        }

        protected abstract void OnInit();
    }
}
