using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MikroFramework.ActionKit;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework
{
    public class RepeatSequence : ActionContainer, IExtensible<RepeatSequence> {
        private int currentRepeatCount = 0;
        private int repeatTime = 1;

        private Sequence repeatSequence;

        private List<MikroAction> repeatActions = new List<MikroAction>();

        public override IEnumerable ActionRecorder {
            get {
                return repeatActions;
            }
        }

        public static RepeatSequence Allocate(int repeatTime=1) {
            RepeatSequence action = SafeObjectPool<RepeatSequence>.Singleton.Allocate();
            action.repeatTime = repeatTime;
            action.repeatSequence=Sequence.Allocate();
            action.repeatSequence.SetAutoRecycle(false);
            return action;
        }

        public override void Reset() {
            base.Reset();
            repeatSequence.Reset();
            repeatSequence.Finished.UnRegisterOnValueChanged(OnRepeatSequenceFinishedValueChanged);
            currentRepeatCount = 0;
            repeatActions.ForEach(action => action.Reset());
        }

        protected override void OnBegin() {
            repeatSequence.Finished.RegisterOnValueChaned(OnRepeatSequenceFinishedValueChanged);
            repeatSequence.Execute();
        }

        private void OnRepeatSequenceFinishedValueChanged(bool value) {
            if (value) {
                currentRepeatCount++;
                //Debug.Log("RepeatSequence:" + currentRepeatCount);
                
                if (currentRepeatCount == repeatTime) {
                    Finished.Value = true;
                }
                else {
                    //restart sequence
                    repeatSequence.Reset();

                    repeatSequence.Execute();
                }
            }
        }

        protected override void SetAutoRecycleForRecordedActions(bool autoRecycle) {
           
        }

        public override void Update() {
            
        }

        protected override void RecycleBackToPool() {
            SafeObjectPool<RepeatSequence>.Singleton.Recycle(this);
        }


        public RepeatSequence AddAction(MikroAction action) {
            repeatSequence.AddAction(action);
            repeatActions.Add(action);
            return this;
        }

        protected override void OnDispose() {
            repeatSequence.Finished.UnRegisterOnValueChanged(OnRepeatSequenceFinishedValueChanged);
            //repeatActions.ForEach((action) => { action.RecycleToCache(); });
            repeatSequence.RecycleToCache();

            repeatActions.Clear();
            repeatSequence = null;
            currentRepeatCount = 0;
            repeatTime = 1;
        }
    }
}
