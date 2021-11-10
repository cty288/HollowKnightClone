using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.ActionKit { 
    public class CallbackAction : MikroAction{
        private Action Callback;


        protected override void OnExecuting() {
            Callback?.Invoke();
            Finished.Value = true;
        }

        public static CallbackAction Allocate(Action callback) {
            CallbackAction callbackAction = SafeObjectPool<CallbackAction>.Singleton.Allocate();
            callbackAction.Callback = callback;
            return callbackAction;
        }

        protected override void RecycleBackToPool() {
            SafeObjectPool<CallbackAction>.Singleton.Recycle(this);
        }
    }
}
