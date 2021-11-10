using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using MikroFramework.Pool;
using MikroFramework.ResKit;
using UnityEngine;

namespace MikroFramework
{
    
    public class PoolManagerWithResLoader : MonoBehaviour {
        private GameObject TestABObj;
        private SafeGameObjectPool spherePool;
        
        //2 ways to load.

        private void Awake() {
            GameObjectPoolManager.AutoAddDefaultPoolableGameObject = true;
            GameObjectPoolManager.AutoCreatePoolWhenAllocating = true;

            //method 1:
            GameObjectPoolManager.Singleton.GetOrCreatePoolFromAB("TestABObj", "mftest",
                out TestABObj);


            //method2:
            spherePool = GameObjectPoolManager.Singleton.GetOrCreatePoolFromAB("Sphere", "test_sphere",
                out GameObject sphere);

            
       
        }

        //method 1:
        public void AllocateAB()
        {
            GameObjectPoolManager.Singleton.Allocate(TestABObj);
        }

        public void RecycleAB()
        {
            GameObjectPoolManager.Singleton.Recycle(GameObject.Find("TestABObj"));
        }


        //method 2:
        public void AllocateSphere() {
            spherePool.Allocate();
        }

        public void RecycleSphere() {
            GameObject.Find("Sphere").GetComponent<PoolableGameObject>().RecycleToCache();
        }

        
    }
}
