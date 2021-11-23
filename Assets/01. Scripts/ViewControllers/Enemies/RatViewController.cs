using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class RatViewController : EnemyBaseViewController<RatConfiguration>
    {
        protected override void Awake() {
            base.Awake();
            Debug.Log(GetCurrentState<SmallAnimalState>() +"    "+ configurationItem.Health +"   "+ Absorbable+"      "+CanAttack);
        }

        protected override void OnFSMStateChanged(string prevEvent, string newEvent) {
            
        }
    }
}
