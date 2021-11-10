using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MikroFramework.Singletons
{
    /// <summary>
    /// Declare a singleton property for a class if the class can't inherit MikroSingleton. (The created instance
    /// have DontDestroyOnLoad
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class SingletonProperty<T> where T: class, ISingleton {
        private static T instance;

        public static T Singleton {
            get {
                if (instance == null) {
                    if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T))) { //is MonoBehavior
                        instance = SingletonCreator.CreateMonoSingleton<T>(true);

                    }else {
                        instance = SingletonCreator.CreateSingleton<T>();
                    }
                }

                return instance;
            }

        }


    }
}
