using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MikroFramework.Singletons {
    /// <summary>
    /// Use MonoMikroSingleton for persistent singletons to inherit MonoBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoPersistentMikroSingleton<T> : MonoMikroSingleton<T>, ISingleton where T : MonoPersistentMikroSingleton<T> {

        protected static T instance=null;

        
        public static T Singleton {
            get
            {
                if (instance == null|| FindObjectOfType<T>()==null) {
                    instance = SingletonCreator.CreateMonoSingleton<T>(true);
                }
                return instance;
            }
        }
        
    }


    /// <summary>
    /// Use MonoMikroSingleton for singletons to inherit MonoBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoMikroSingleton<T> : MikroBehavior,ISingleton where T : MonoMikroSingleton<T>
    {

        protected static T instance = null;
        /// <summary>
        /// Get if the singleton exists in the game
        /// </summary>
        public static bool Exists
        {
            get
            {
                return FindObjectOfType<T>()!=null;
            }
        }

        public static T Singleton
        {
            get
            {
                if (instance == null || FindObjectOfType<T>()==null) {
                    instance = SingletonCreator.CreateMonoSingleton<T>(false);
                }
                return instance;
            }
        }
        

        protected override void OnBeforeDestroy() {
            instance = null;
        }

        public virtual void OnSingletonInit() {
            
        }

        protected static bool onApplicationQuit = false;
        public static bool IsApplicationQuit {
            get { return onApplicationQuit; }
        }

        private void OnApplicationQuit() {
            onApplicationQuit = true;
            if (instance != null) {
                Destroy(instance.gameObject);
                instance = null;
            }
        }
        
    }

}
