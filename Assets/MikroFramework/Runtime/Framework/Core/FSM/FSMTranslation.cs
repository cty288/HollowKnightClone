using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.FSM
{

    public class FSMTranslation: IPoolable {
        public FSMState fromState;
        public string name;
        public FSMState toState;


        /// <summary>
        /// Callback function
        /// </summary>
        public FSM.FSMTranslationCallback callback;

        public FSMTranslation() { }

        public void OnRecycled() {
            fromState = null;
            name = null;
            toState = null;
        }

        public bool IsRecycled { get; set; }
        public void RecycleToCache() {
            SafeObjectPool<FSMTranslation>.Singleton.Recycle(this);
        }

        /// <summary>
        /// Allocate a FSMTranslation object 
        /// </summary>
        /// <param name="fromState">Previous state</param>
        /// <param name="name">Translation name (or the event name that switch from the previous state to the new state)</param>
        /// <param name="toState">new state</param>
        /// <param name="callback">callback function when switched</param>
        /// <returns></returns>
        public static FSMTranslation Allocate(FSMState fromState, string name, FSMState toState,
            FSM.FSMTranslationCallback callback) {

            FSMTranslation retResult = SafeObjectPool<FSMTranslation>.Singleton.Allocate();
            retResult.fromState = fromState;
            retResult.name = name;
            retResult.toState = toState;
            retResult.callback = callback;

            return retResult;
        }
    }
}
