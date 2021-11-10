using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework
{

    public static class DelayActionExtensions {
        public static void Delay(this MonoBehaviour self, float seconds, Action onDelayFinished) {
            self.Execute(DelayAction.Allocate(seconds,onDelayFinished));
        }
    }

    public class DelayAction : MikroAction {
        private float delayTime = 0;
        private float currentTime = 0;
        private Action onDelayFinished;

        public static DelayAction Allocate(float delayTime, Action onDelayFinished=null) {
            DelayAction delayAction = SafeObjectPool<DelayAction>.Singleton.Allocate();
            delayAction.delayTime = delayTime;
            delayAction.onDelayFinished = onDelayFinished;
            delayAction.currentTime = 0;
            return delayAction;
        }

        public override void Reset() {
            base.Reset();
            currentTime = 0;
        }

        protected override void OnExecuting() {
            currentTime += Time.deltaTime;
            if (currentTime >= delayTime)
            {
                onDelayFinished?.Invoke();
                Finished.Value = true;
            }
        }

       

        protected override void RecycleBackToPool() {
            SafeObjectPool<DelayAction>.Singleton.Recycle(this);
        }

    }
}
