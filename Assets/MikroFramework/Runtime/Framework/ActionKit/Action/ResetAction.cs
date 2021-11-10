using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework
{
    public class ResetAction : MikroAction {
        private MikroAction resetAction;
        private Action onReset;

        protected override void OnExecuting() {
            resetAction.Reset();
            
            Finished.Value = true;
            onReset?.Invoke();
        }

        public static ResetAction Allocate(MikroAction resetAction, Action onReset=null) {
            ResetAction action = SafeObjectPool<ResetAction>.Singleton.Allocate();
            action.resetAction = resetAction;
            action.onReset = onReset;
            return action;
        }


        protected override void RecycleBackToPool() {
            SafeObjectPool<ResetAction>.Singleton.Recycle(this);
        }
    }
}
