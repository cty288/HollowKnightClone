using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using MikroFramework.Singletons;
using MikroFramework.Utilities;
using UnityEngine;

namespace MikroFramework
{
    public class GameObjectPoolManager : MonoMikroSingleton<GameObjectPoolManager>,
        IGameObjectPoolManager<SafeGameObjectPool>
    {

        private Dictionary<string, SafeGameObjectPool> pools;
        public Dictionary<string, SafeGameObjectPool> GameObjectPools
        {
            get { return pools; }
        }

        private ResLoader resLoader;
        /// <summary>
        /// If the pooled gameobject does not contain a component that inherits from PoolableGameObject class,
        /// will the manager auto add a default PoolableGameObject component to it? If not, an error will be
        /// thrown if the pooled gameobject does not have the component. (Default: false)
        /// </summary>
        public static bool AutoAddDefaultPoolableGameObject = false;

        /// <summary>
        /// When requesting the manager to allocate a prefab, if the Manager does not contain a
        /// GameObjectPool for that prefab, will the Manager auto create one for it? If not, an error will be
        /// thrown if the Manager does not contain it. (Default: false)
        /// </summary>
        public static bool AutoCreatePoolWhenAllocating = false;

        private void Awake()
        {
            pools = new Dictionary<string, SafeGameObjectPool>();
            resLoader = new ResLoader();
        }

        public SafeGameObjectPool GetOrCreatePool(GameObject prefab)
        {
            return CreatePool(prefab, 10, 50);
        }

        public SafeGameObjectPool GetOrCreatePoolFromAB(string prefabAssetName, string ownerBundleName,
            out GameObject prefab)
        {
            return CreatePoolFromAB(prefabAssetName, ownerBundleName, 10, 50, out prefab);
        }

        public SafeGameObjectPool CreatePool(GameObject prefab, int initialCount, int maxCount)
        {
            string prefabName = prefab.name;

            if (CheckPoolExists(prefab,out SafeGameObjectPool existingPool))
            {
                return existingPool;
            }

            if (AutoAddDefaultPoolableGameObject)
            {
                if (prefab.GetComponent<PoolableGameObject>() == null)
                {
                    prefab.AddComponent<DefaultPoolableGameObject>();
                }
            }

            SafeGameObjectPool pool = GameObjectPool.Create(prefab).Init(initialCount, maxCount);
            GameObjectPools.Add(prefabName, pool);
            return pool;
        }

        public SafeGameObjectPool CreatePoolFromAB(string prefabAssetName, string ownerBundleName,
            int initialCount, int maxCount, out GameObject prefab)
        {
            prefab = resLoader.LoadSync<GameObject>(ownerBundleName, prefabAssetName);
            return CreatePool(prefab, initialCount, maxCount);
        }

        private bool CheckPoolExists(GameObject prefab, out SafeGameObjectPool pool) {
            string prefabName = prefab.name;
            if (!pools.ContainsKey(prefabName)) {
                SafeGameObjectPool existingPool = GameObject.Find($"Object Pool: {prefabName}").GetComponent<SafeGameObjectPool>();
                if (existingPool) {
                    pools.Add(prefabName,existingPool);
                    pool = existingPool;
                    return true;
                }

                pool = null;
                return false;
            }

            pool = pools[prefabName];
            return true;
        }


        public GameObject Allocate(GameObject prefab)
        {
            if (prefab) {
                SafeGameObjectPool pool;
                string prefabName = prefab.name;

                if (!CheckPoolExists(prefab,out pool))
                {
                    if (AutoCreatePoolWhenAllocating)
                    {
                        GetOrCreatePool(prefab);
                    }
                    else
                    {
                        Debug.LogError(
                            $"The pool does not exist for {prefabName}! Use CreatePool() to create its pool!");
                        return null;
                    }
                }


                return pool.Allocate();

            }

            Debug.LogWarning("A Prefab is missing but the SafeGameObjectPool is still trying to access it!");
            return null;
        }



        public bool Recycle(GameObject recycledObject)
        {
            if (recycledObject)
            {
                string prefabName = CommonUtility.DeleteCloneName(recycledObject);
                recycledObject.name = prefabName;
                SafeGameObjectPool pool;

                if (!CheckPoolExists(recycledObject,out pool))
                {
                    Debug.LogError($"The pool does not exist for {prefabName}! Use CreatePool() to create its pool!");
                    return false;
                }
                else
                {
                    return pool.Recycle(recycledObject);
                }
            }

            return false;
        }

        public bool AddNewPool(SafeGameObjectPool newPool)
        {
            if (newPool != null && newPool.PooledPrefab)
            {
                if (!CheckPoolExists(newPool.PooledPrefab,out SafeGameObjectPool pool))
                {
                    pools.Add(newPool.PooledPrefab.name, newPool);
                    return true;
                }
            }

            return false;
        }

        protected override void OnBeforeDestroy()
        {
            base.OnBeforeDestroy();
            resLoader.ReleaseAllAssets();
        }
    }
}
