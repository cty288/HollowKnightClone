 using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MikroFramework.Singletons {
    /// <summary>
    /// Inherit MikroSingleton for singletons that do not need to inherit MonoBehavior;
    /// Use MonoMikroSingleton for singletons to inherit MonoBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MikroSingleton<T>: ISingleton where T: MikroSingleton<T> {
        protected static T instance;
        public static T Singleton {
            get {
                if (instance == null) {
                    instance = SingletonCreator.CreateSingleton<T>();
                }

                return instance;
            }
        }

        public virtual void OnSingletonInit() {
            
        }
    }

}
