using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Pool
{
    public interface IGameObjectPoolManager<T> where T:GameObjectPool {
        /// <summary>
        /// A dictionary of all pools in the current scene
        /// </summary>
        Dictionary<string, T> GameObjectPools { get;  }

        /// <summary>
        /// Get or Create a GameObject Pool, given a prefab. The default pool created will have
        /// 10 initial objects, and 50 as its max count.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        T GetOrCreatePool(GameObject prefab);

        /// <summary>
        /// Get or Create a GameObject pool, in which its prefab is loaded from AssetBundle. Out returns the prefab from
        /// the AB
        /// </summary>
        /// <param name="prefabAssetName"></param>
        /// <param name="ownerBundleName"></param>
        /// <returns></returns>
        T GetOrCreatePoolFromAB(string prefabAssetName, string ownerBundleName, out GameObject prefab);


        /// <summary>
        /// Create a new GameObject pool, given a prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="initialCount"></param>
        /// <param name="maxCount">The max number of objects in the pool. Objects will be destroyed when recycling if the pool is full</param>
        /// <returns></returns>
        T CreatePool(GameObject prefab, int initialCount, int maxCount);

        /// <summary>
        /// Create a new GameObject pool, in which its prefab is loaded from AssetBundle. Out Returns the prefab from
        /// the AB
        /// </summary>
        /// <param name="prefabAssetName"></param>
        /// <param name="ownerBundleName"></param>
        /// <returns></returns>
        T CreatePoolFromAB(string prefabAssetName, string ownerBundleName, int initialCount,
            int maxCount, out GameObject prefab);

        /// <summary>
        /// Allocate a GameObject from its corresponding pool. Throw error if the pool does not exist
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        GameObject Allocate(GameObject prefab);


        /// <summary>
        /// Recycle the GameObject back to its pool. Throw error if the pool does not exist
        /// </summary>
        /// <param name="recycledObject"></param>
        /// <returns></returns>
        bool Recycle(GameObject recycledObject);

        
        /// <summary>
        /// Join a new pool to the existing PoolManager. Return false if the pool already exist
        /// </summary>
        /// <param name="newPool"></param>
        /// <returns></returns>
        bool AddNewPool(T newPool);
    }
}
