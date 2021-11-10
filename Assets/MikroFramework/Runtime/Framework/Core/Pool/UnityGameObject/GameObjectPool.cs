using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Factory;
using MikroFramework.Pool;
using MikroFramework.Singletons;
using MikroFramework.Utilities;
using UnityEngine;

namespace MikroFramework.Pool
{
    public abstract class GameObjectPool : MonoBehaviour, IPool<GameObject>
    {
        public enum GameObjectPoolState
        {
            NotInited,
            Initializing,
            Inited
        }

        protected GameObjectPoolState poolState = GameObjectPoolState.NotInited;
        protected Stack<GameObject> cachedStack = new Stack<GameObject>();
        protected IObjectFactory<GameObject> prefabFactory;

        [SerializeField]
        protected GameObject pooledPrefab;
        public GameObject PooledPrefab => pooledPrefab;

        /// <summary>
        /// Create a new GameObject Pool, given the type of the GameObject Pool
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pooledPrefab"></param>
        /// <returns></returns>
        public static T Create<T>(GameObject pooledPrefab) where T : GameObjectPool
        {
            T pool = new GameObject().AddComponent<T>();
            pool.Init(pooledPrefab);
            return pool;
        }

        /// <summary>
        /// Create a new SafeGameObject Pool.
        /// </summary>
        /// <param name="pooledPrefab"></param>
        /// <returns></returns>
        public static SafeGameObjectPool Create(GameObject pooledPrefab)
        {
            return Create<SafeGameObjectPool>(pooledPrefab);
        }

        public int CurrentCount
        {
            get { return cachedStack.Count; }
        }

        public virtual GameObject Allocate()
        {
            if (pooledPrefab != null)
            {
                GameObject allocatedObj;
                if (poolState == GameObjectPoolState.NotInited || poolState == GameObjectPoolState.Initializing)
                {
                    allocatedObj = prefabFactory.Create();
                }
                else
                {
                    allocatedObj = cachedStack.Count > 0 ? cachedStack.Pop() : prefabFactory.Create();
                }

                allocatedObj.name = CommonUtility.DeleteCloneName(allocatedObj);

                return allocatedObj;
            }

            return null;


        }

        public abstract bool Recycle(GameObject obj);

        protected abstract void Init(GameObject pooledPrefab);

    }
}
