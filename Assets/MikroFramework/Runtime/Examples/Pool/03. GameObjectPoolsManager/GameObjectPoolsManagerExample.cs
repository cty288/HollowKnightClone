using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MikroFramework.Examples
{
    public class GameObjectPoolsManagerExample : MonoBehaviour {
        public GameObject cubePrefab;
        public GameObject spherePrefab;
        public GameObject CylinderPrefab;

        private void Awake() {
            GameObjectPoolManager.Singleton.GetOrCreatePool(cubePrefab);
            GameObjectPoolManager.Singleton.CreatePool(spherePrefab, 5, 10);
            GameObjectPoolManager.Singleton.CreatePool(CylinderPrefab, 0, 5);
        }

        public void Allocate(GameObject prefab) {
            GameObjectPoolManager.Singleton.Allocate(prefab);
        }

        public void Recycle(GameObject prefab) {
            GameObject.Find(prefab.name).GetComponent<PoolableGameObject>().RecycleToCache();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene("PoolManagerScene2");
            }
        }
    }
}
