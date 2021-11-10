using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.ActionKit;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public class Spawn : ActionContainer, IExtensible<Spawn> {
        private List<MikroAction> recordedActions = new List<MikroAction>();
        private List<MikroAction> actions = new List<MikroAction>();
        private List<MikroAction> executingActions = new List<MikroAction>();

        public override IEnumerable ActionRecorder {
            get {
                return recordedActions;
            }
        }

        public override void Update() {
            foreach (MikroAction mikroAction in actions) {
                ActionPlayer.Singleton.Play(mikroAction);
                executingActions.Add(mikroAction);
            }
            actions.Clear();

            Finished.Value = executingActions.All(action => action.Finished.Value);
        }

        public Spawn AddAction(MikroAction action) {
            actions.Add(action);
            recordedActions.Add(action);
            action.SetAutoRecycle(AutoRecycle);
            return this;
        }

        
        protected override void SetAutoRecycleForRecordedActions(bool autoRecycle) {
            recordedActions.ForEach(action => action.SetAutoRecycle(autoRecycle));
        }

        public static Spawn Allocate() {
            return SafeObjectPool<Spawn>.Singleton.Allocate();
        }

        protected override void OnDispose() {
            actions.Clear();
            executingActions.Clear();
            recordedActions.ForEach(action=>action.RecycleToCache());
            recordedActions.Clear();
        }

        public override void Reset() {
            base.Reset();
            actions.Clear();
            executingActions.Clear();
            recordedActions.ForEach(action=>action.Reset());
        }

        protected override void RecycleBackToPool() {
            SafeObjectPool<Spawn>.Singleton.Recycle(this);
        }
    }
}
