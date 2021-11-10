using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework
{
    public class UntilAction : MikroAction {
        private Func<bool> triggerCondition;

        public static UntilAction Allocate(Func<bool> triggerCondition) {
            UntilAction action = SafeObjectPool<UntilAction>.Singleton.Allocate();
            action.triggerCondition = triggerCondition;
            return action;
        }


        protected override void OnExecuting() {
            if(triggerCondition!=null)
                Finished.Value = triggerCondition.Invoke();
        }

        protected override void RecycleBackToPool() {
            SafeObjectPool<UntilAction>.Singleton.Recycle(this);
        }

        protected override void OnDispose() {
            triggerCondition = null;
        }
    }
}
