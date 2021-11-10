using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public class Sequence : ActionContainer, IExtensible<Sequence> {
        private List<MikroAction> actions = new List<MikroAction>();

        private List<MikroAction> recordedActions = new List<MikroAction>();
        private MikroAction currentAction = null;

        public override IEnumerable ActionRecorder {
            get {
                return recordedActions;
            }
        }

        public override void Update() {
            //has actions
            if (actions.Any()) {
                
                //current action finished in last frame -> remove it from the top actions
                if (currentAction != null && currentAction.Finished.Value) {
                    actions.RemoveAt(0);
                }

                //first run or last action has finished -> play the top action, and save to currentAction
                if (actions.Any() && (currentAction == null || currentAction.Finished.Value)) {
                    currentAction = actions.First();
                    ActionPlayer.Singleton.Play(currentAction);
                }
                
            }
            else {
                Finished.Value = true;
            }
        }

        public override void Reset() {
            base.Reset();
            actions.Clear();
            recordedActions.ForEach(action=>action.Reset());
            actions.AddRange(recordedActions);
            currentAction = null;
        }

        /// <summary>
        /// Add an action to the sequence, in which each action in the sequence will be played in sequence each frame
        /// </summary>
        /// <param name="action"></param>
        public Sequence AddAction(MikroAction action) {
            actions.Add(action);
            recordedActions.Add(action);
            action.SetAutoRecycle(AutoRecycle);
            return this;
        }

        public Sequence AddAction(List<MikroAction> actions) {
            this.actions.AddRange(actions);
            recordedActions.AddRange(actions);
            actions.ForEach(action=>action.SetAutoRecycle(AutoRecycle));
            return this;
        }

       

        protected override void SetAutoRecycleForRecordedActions(bool autoRecycle) {
            recordedActions.ForEach(action => action.SetAutoRecycle(autoRecycle));
        }

        public static Sequence Allocate() {
            return SafeObjectPool<Sequence>.Singleton.Allocate();
        }

        protected override void OnDispose() {
            actions.Clear();
            recordedActions.ForEach(action=>action.RecycleToCache());
            recordedActions.Clear();
            currentAction = null;
        }

        protected override void RecycleBackToPool() {
            SafeObjectPool<Sequence>.Singleton.Recycle(this);
        }
    }
}
