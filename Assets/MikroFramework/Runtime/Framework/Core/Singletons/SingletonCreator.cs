using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MikroFramework.Singletons
{
    /// <summary>
    /// Helper class, create non-mono and mono singletons
    /// </summary>
    public static class SingletonCreator {
        public static T CreateSingleton<T>() where T : class, ISingleton {
            var creators = typeof(T).GetConstructors(BindingFlags.Instance |
                                                     BindingFlags.NonPublic);
            var creator = Array.Find(creators, c => c.GetParameters().Length == 0);

            if (creator == null)
            {
                throw new Exception("Non-public ctor() not found!");
            }

            var  instance = creator.Invoke(null) as T;
            instance.OnSingletonInit();
            return instance;
        }

        public static T CreateMonoSingleton<T>(bool dontDestroyOnLoad) where T : class, ISingleton {
            if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T))) {
                //is MonoBehavior
                var instance = Object.FindObjectOfType(typeof(T)) as T;

                if (Object.FindObjectsOfType(typeof(T)).Length > 1)
                {
                    Debug.LogWarning($"More than one singleton {typeof(T).Name} exists in the current scene!");
                    return instance;
                }

                if (instance != null) {
                    return instance;
                }

                Type info = typeof(T);

                instance = info.GetCustomAttributes(false).Cast<MonoSingletonPath>().Select(monoSingletonPath =>
                    CreateMonoSingletonWithPath<T>(monoSingletonPath.PathInHierarchy, dontDestroyOnLoad)).FirstOrDefault();

                if (instance == null)
                {

                    GameObject instanceGameObject = GetOrCreateInstanceGo<T>();
                    instance = instanceGameObject.AddComponent(typeof(T)) as T;
                    if (dontDestroyOnLoad)
                    {
                        Object.DontDestroyOnLoad(instanceGameObject);
                    }
                    Debug.Log($"Added new singleton {typeof(T).Name} to the game!");

                }
                else
                {
                    Debug.Log($"{instance.GetType()} already exists!");
                }

                instance.OnSingletonInit();
                return instance;
            }

            throw new Exception($"Type {typeof(T)} is not a MonoBehavior script!");

        }

        private static GameObject GetOrCreateInstanceGo<T>()
        {
            string instanceName = typeof(T).Name;
            GameObject instanceGameObject = GameObject.Find(instanceName);

            if (!instanceGameObject)
            {
                instanceGameObject = new GameObject(instanceName);
            }

            return instanceGameObject;
        }

        private static T CreateMonoSingletonWithPath<T>(string path, bool dontDestroy) where T : class, ISingleton {
            GameObject gameObject = GetOrCreateMonoGameObjectWithPath(path, true, dontDestroy);

            if (!gameObject) {
                gameObject = new GameObject("Singleton of " + typeof(T).Name);

                if (dontDestroy) {
                    Object.DontDestroyOnLoad(gameObject);
                }
            }

            return gameObject.AddComponent(typeof(T)) as T;
        }

        private static GameObject GetOrCreateMonoGameObjectWithPath(string path, bool build, bool dontDestroy) {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }

            string[] subPath = path.Split('/');

            if (subPath.Length == 0) {
                return null;
            }

            return GetOrCreateMonoGameObjectWithPathArray(null, subPath, 0, build, dontDestroy);
        }

        /// <summary>
        /// Recursively find the leaf GameObject
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subpath"></param>
        /// <param name="index"></param>
        /// <param name="build"></param>
        /// <param name="dontDestroy"></param>
        /// <returns></returns>
        private static GameObject GetOrCreateMonoGameObjectWithPathArray(GameObject parent, string[] subpath,
            int index, bool build, bool dontDestroy) {

            while (true) {
                GameObject currentObj = null;

                if (!parent) {
                    currentObj=GameObject.Find(subpath[index]);
                }
                else {
                    Transform child = parent.transform.Find(subpath[index]);
                    if (child != null) {
                        currentObj = child.gameObject;
                    }
                }

                if (!currentObj) {
                    if (build) {
                        currentObj = new GameObject(subpath[index]);

                        if (parent != null) {
                            currentObj.transform.SetParent(parent.transform);
                        }

                        if (dontDestroy && index == 0) {
                            Object.DontDestroyOnLoad(currentObj);
                        }
                    }
                }

                if (!currentObj) {
                    return null;
                }

                if (++index == subpath.Length) {
                    return currentObj;
                }

                parent = currentObj;
            }
        }
    }
}

