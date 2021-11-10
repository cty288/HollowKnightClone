using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public abstract class MikroAction: ICommand, IPoolable {
        public BindableProperty<bool> Finished = new BindableProperty<bool>(){Value = false};

        public Action OnEndedCallback;
        public Action OnBeginCallback;

        private IArchitecture architectureModel;
        public void Execute() {
            Finished.Value = false;
            Finished.RegisterOnValueChaned(OnFinishedValueChanged);

            if (Finished.Value) {
                return;
            }

            OnBeginCallback?.Invoke();
            OnBegin();
            Executing();

            if (!Finished.Value && !IsRecycled) {
                //ActionPlayer.Singleton.UnRegisterUpdate(Executing);
                
                ActionPlayer.Singleton.RegisterUpdate(Executing);
            }
            
        }

        /// <summary>
        /// Reset is different from Dispose. Reset means reset certain properties back to its initial value, but
        /// not dispose properties to null. When AutoRecycle is turned off, you can Reset this action and
        /// Execute it again.
        /// </summary>
        public virtual void Reset() {
            if (!Finished.Value) {
                ActionPlayer.Singleton.UnRegisterUpdate(Executing);
            }
           
            Finished.Value = false;
        }

        protected void Finish() {
            Finished.Value = true;
        }

        /// <summary>
        /// OnBegin is called right after executing this action, and will be only called once
        /// </summary>
        protected virtual void OnBegin() {
            
        }

        private void Executing() {
            if (!Finished.Value) {
                OnExecuting();
            }
        }

        /// <summary>
        /// OnExecuting is called each frame after executing this action, until this action is finished
        /// </summary>
        protected virtual void OnExecuting() {
            
        }

        /// <summary>
        /// OnEnd is called once before this action ends
        /// </summary>
        protected virtual void OnEnd() {

        }

        /// <summary>
        /// OnDispose is called before this action is recycled back to the pool (before reset everything)
        /// </summary>
        protected virtual void OnDispose() {

        }

        protected bool AutoRecycle = true;

        private void OnFinishedValueChanged(bool finished) {
            if (finished) {
                ActionPlayer.Singleton.UnRegisterUpdate(Executing);
                OnEnd();
                OnEndedCallback?.Invoke();
                Finished.UnRegisterOnValueChanged(OnFinishedValueChanged);
                if (AutoRecycle) {
                    RecycleToCache();
                }
            }
        }

        void IPoolable.OnRecycled() { }

        public bool IsRecycled { get; set; }

        /// <summary>
        /// Auto recycle after finished. If false, you will need to manually recycle the action by calling RecycleToCache()
        /// </summary>
        /// <param name="autoRecycle"></param>
        public virtual void SetAutoRecycle(bool autoRecycle) {
            this.AutoRecycle = autoRecycle;
        }

        /// <summary>
        /// Auto invoked after end
        /// </summary>
        public void RecycleToCache() {
            OnDispose();
            //Reset();
            AutoRecycle = true;
            OnEndedCallback = null;
            OnBeginCallback = null;
            RecycleBackToPool();
        }

        protected abstract void RecycleBackToPool();

        public IArchitecture GetArchitecture() {
            return architectureModel;
        }

        public void SetArchitecture(IArchitecture architecture) {
            this.architectureModel = architecture;
        }
    }
}
