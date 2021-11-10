using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public abstract class ActionContainer: MikroAction
    {
        public abstract IEnumerable ActionRecorder { get; }

        public override void SetAutoRecycle(bool autoRecycle) {
            base.SetAutoRecycle(autoRecycle);
            SetAutoRecycleForRecordedActions(autoRecycle);
        }

        protected abstract void SetAutoRecycleForRecordedActions(bool autoRecycle);

        protected override void OnExecuting() {
            Update();
        }

        public abstract void Update();
    }
}
