using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.FSM
{
    public class FSMState:IPoolable {
        public string name;

        public FSMState() { }

        public FSMState(string name) {
            this.name = name;
        }

        /// <summary>
        /// Allocate a new FSM State
        /// </summary>
        /// <param name="name">Name of this state</param>
        /// <returns></returns>
        public static FSMState Allocate(string name) {
            FSMState retResult = SafeObjectPool<FSMState>.Singleton.Allocate();
            retResult.name = name;
            return retResult;
        }

       
        private Dictionary<string, FSMTranslation> translationDictionary = new Dictionary<string, FSMTranslation>();
        /// <summary>
        /// Translation for this state. "name" is translation's name (or the event name that switch from this state to next state)
        /// </summary>
        public Dictionary<string, FSMTranslation> TranslationDictionary => translationDictionary;


        /// <summary>
        /// Add a translation for this state
        /// </summary>
        /// <param name="translation"></param>
        public void AddTranslation(FSMTranslation translation) {
            translationDictionary[translation.name] = translation;
        }

        public void OnRecycled() {
            //recycle all translations
            var valueEnumerator = translationDictionary.Values.GetEnumerator();
            //recycle all states
            while (valueEnumerator.MoveNext())
            {
                FSMTranslation translation = valueEnumerator.Current;
                if (translation != null) {
                    translation.RecycleToCache();
                }
               
            }

            translationDictionary.Clear();
            valueEnumerator.Dispose();
        }

        public bool IsRecycled { get; set; }
        public void RecycleToCache() {
            SafeObjectPool<FSMState>.Singleton.Recycle(this);
        }
    }
}
