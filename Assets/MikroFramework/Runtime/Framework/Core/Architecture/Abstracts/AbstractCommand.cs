using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.Architecture
{
    /// <summary>
    /// Non-async command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractCommand<T> : ICommand where T : AbstractCommand<T>, new()
    {
        private IArchitecture architectureModel;
        protected virtual bool AutoRecycle { get; } = true;

        IArchitecture IBelongToArchitecture.GetArchitecture()
        {
            return architectureModel;
        }

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture)
        {
            this.architectureModel = architecture;
        }


        void ICommand.Execute()
        {
            OnExecute();
            if (AutoRecycle)
            {
                RecycleToCache();
            }

        }

        /// <summary>
        /// Execute this command
        /// </summary>
        /// <param name="parameters"></param>
        protected abstract void OnExecute();

        public virtual void OnRecycled()
        {
            architectureModel = null;
        }

        public bool IsRecycled { get; set; }

        public void RecycleToCache()
        {
            SafeObjectPool<T>.Singleton.Recycle(this as T);
        }


    }
}