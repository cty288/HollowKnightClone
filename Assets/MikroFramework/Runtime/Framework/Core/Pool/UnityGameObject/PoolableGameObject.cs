using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Pool
{
    public abstract class PoolableGameObject:MonoBehaviour {

        /// <summary>
        /// The pool that belongs to this object
        /// </summary>
        public SafeGameObjectPool Pool;

        /// <summary>
        /// Call this method to recycle this gameobject back to its pool
        /// </summary>
        public void RecycleToCache() {
            Pool.Recycle(this.gameObject);
            Pool = null;
        }

        
        /// <summary>
        /// Triggered after recycled back to the pool, or after calling Recycle()
        /// </summary>
        public abstract void OnRecycled();
    }
}
