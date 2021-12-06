using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public abstract class AbstractUltCommand<T> : AbstractWeaponCommand<T>, IWeaponCommand where T : AbstractUltCommand<T>, new() {
        private ITimeSystem timeSystem;

        
        protected override void OnExecute() {
            if (timeSystem == null)
            {
                timeSystem = this.GetSystem<ITimeSystem>();
                this.GetModel<IPlayerModel>().UltChargeAccumlated.Value = 0;
                OnUltExecuted();
            }
        }

        protected abstract void OnUltExecuted();
    }
}
