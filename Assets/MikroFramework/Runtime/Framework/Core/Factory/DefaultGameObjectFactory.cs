using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Factory
{
    /// <summary>
    /// A default factory for creating unity GameObjects. Prefab will be instantiated to the origin of the game
    /// </summary>
    public class DefaultGameObjectFactory : IObjectFactory<GameObject> {
        private GameObject prefab;

        public DefaultGameObjectFactory(GameObject prefab) {
            this.prefab = prefab;
        }

        public GameObject Create() {
            if (prefab != null) {
                return GameObject.Instantiate(prefab);
            }

            return null;
        }
    }
}
