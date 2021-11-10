using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public abstract class AbstractModel : IModel {
        private IArchitecture architectureModel;

        IArchitecture IBelongToArchitecture.GetArchitecture() {
            return architectureModel;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) {
            this.architectureModel = architecture;
        }

        void IModel.Init() {
            OnInit();
        }


        protected abstract void OnInit();
    }
}
