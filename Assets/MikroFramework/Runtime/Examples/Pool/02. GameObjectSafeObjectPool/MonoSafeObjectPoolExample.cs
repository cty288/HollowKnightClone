using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class MonoSafeObjectPoolExample : MonoBehaviour
    {

        List<PoolableGameObject> allocatedObjs = new List<PoolableGameObject>();

        private SafeGameObjectPool gameObjectPool;
        public GameObject PooledGameObjectPrefab1;

        private void Start() {
            gameObjectPool = GameObjectPool.Create<SafeGameObjectPool>(PooledGameObjectPrefab1).Init(5, 15);


            //Thread.Sleep(1000);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.D)) {

                foreach (PoolableGameObject allocatedObj in allocatedObjs) {
                    allocatedObj.RecycleToCache();
                }

                allocatedObjs.Clear();
            }

            if (Input.GetKeyDown(KeyCode.A)) {

                GameObject obj = gameObjectPool.Allocate();
                allocatedObjs.Add(obj.GetComponent<PoolableGameObject>());
                Debug.Log(gameObjectPool.CurrentCount);
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                gameObjectPool.MaxCount = 10;
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                allocatedObjs[0].RecycleToCache();
                allocatedObjs.RemoveAt(0);
            }
        }
    }

   
}
