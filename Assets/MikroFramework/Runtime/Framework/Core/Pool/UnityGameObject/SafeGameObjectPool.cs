using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using MikroFramework.Factory;
using UnityEngine;

namespace MikroFramework.Pool
{
    //TODO: add extension
    public class SafeGameObjectPool : GameObjectPool
    {

        [SerializeField]
        protected int initCount = 0;
        [SerializeField]
        protected int maxCount = 50;

        protected Queue<GameObject> destroyedObjectInQueue;

        private int numHiddenObjectCreating = 0;



        /// <summary>
        /// Number of Object Instantitated to the Object pool per frame at the initialization process of the object pool
        /// </summary>
        public float NumObjInitPerFrame = 10;

        /// <summary>
        /// Number of Object Destroyed from the Object pool per frame
        /// </summary>
        public float NumObjDestroyPerFrame = 2;

        public int MaxCount
        {
            get
            {
                return maxCount;
            }
            set
            {
                CheckInited();
                if (value < 0)
                {
                    Debug.LogError("MaxCount must be greater or equal to 0");
                }
                maxCount = value;

                if (maxCount < CurrentCount)
                {
                    int removedCount = CurrentCount - maxCount;
                    for (int i = 0; i < removedCount; i++)
                    {
                        GameObject popedObj = cachedStack.Pop();

                        if (!popedObj.activeInHierarchy)
                        {
                            destroyedObjectInQueue.Enqueue(popedObj);
                            destroyingObjs.Value = true;
                        }
                    }
                }
            }
        }

        private void Awake()
        {
            creatingObjs.RegisterOnValueChaned(value => {
                if (value)
                {
                    StartCoroutine(InitializeObjectsToGame());
                }
            }).UnRegisterWhenGameObjectDestroyed(this.gameObject);

            destroyingObjs.RegisterOnValueChaned(value => {
                if (value)
                {
                    StartCoroutine(DestoryObjectsInQueue());
                }
            }).UnRegisterWhenGameObjectDestroyed(this.gameObject);
        }

        /// <summary>
        /// Recycle and disactive the gameobject. 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Recycle(GameObject obj)
        {
            CheckInited();

            if (obj && pooledPrefab)
            {
                if (obj.name != pooledPrefab.name)
                {
                    Debug.LogError($"{obj.name} recycled to the wrong ObjectPool!");
                }

                if (obj == null || !obj.activeInHierarchy)
                {
                    return false;
                }

                if (CurrentCount < maxCount)
                {
                    obj.SetActive(false);
                    obj.GetComponent<PoolableGameObject>().OnRecycled();
                    cachedStack.Push(obj);
                    return true;
                }
                else
                {
                    Debug.Log($"The SafeGameObjectPool {pooledPrefab.name} is full! The Object will not return to the pool");
                    obj.SetActive(false);
                    destroyedObjectInQueue.Enqueue(obj);
                    destroyingObjs.Value = true;
                    obj.GetComponent<PoolableGameObject>().OnRecycled();
                    return false;
                }
            }
            else
            {
                Debug.LogWarning("A Prefab is missing but the SafeGameObjectPool is still trying to access it!");
                return false;
            }


        }

        /// <summary>
        /// Allocate a gameobject from the pool and set active
        /// </summary>
        /// <returns></returns>
        public override GameObject Allocate()
        {
            CheckInited();
            GameObject createdObj = base.Allocate();
            if (createdObj)
            {
                createdObj.transform.SetParent(this.transform);
                createdObj.SetActive(true);
                createdObj.GetComponent<PoolableGameObject>().Pool = this;
            }
            else
            {
                Debug.LogWarning("A Prefab is missing but the SafeGameObjectPool is still trying to access it!");
            }

            return createdObj;
        }

        protected override void Init(GameObject pooledPrefab)
        {
            destroyedObjectInQueue = new Queue<GameObject>(maxCount);
            Init(pooledPrefab, 0, 50);
        }


        /// <summary>
        /// Initialize the initial count and max count of the pool. 
        /// </summary>
        /// <param name="initCount"></param>
        /// <param name="maxCount"></param>
        public SafeGameObjectPool Init(int initCount = 0, int maxCount = 50)
        {
            Init(pooledPrefab, initCount, maxCount);
            return this;
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

        protected void Init(GameObject pooledPrefab, int initCount = 0, int maxCount = 50)
        {
            cachedStack.Clear();
            destroyedObjectInQueue.Clear();
            numHiddenObjectCreating = 0;


            if (pooledPrefab.GetComponent<PoolableGameObject>() == null)
            {
                Debug.LogError("Pooled Prefab must have a component that inherited from PoolableGameObject!");
            }

            this.gameObject.name = $"Object Pool: {pooledPrefab.name}";
            this.pooledPrefab = pooledPrefab;
            this.initCount = initCount;
            this.maxCount = maxCount;

            prefabFactory = new DefaultGameObjectFactory(pooledPrefab);

            if (initCount > maxCount)
            {
                Debug.LogError("Initial count of the SafeObjectPool can't be bigger than the maxCount");
            }

            if (initCount < 0 || maxCount < 0)
            {
                Debug.LogError("Initial count or Max Count must be greater or equal to 0");
            }

            this.maxCount = maxCount;

            if (CurrentCount < initCount)
            {
                numHiddenObjectCreating += initCount - CurrentCount;
                poolState = GameObjectPoolState.Initializing;
                creatingObjs.Value = true;
            }
            else
            {
                poolState = GameObjectPoolState.Inited;
            }


        }


        private BindableProperty<bool> creatingObjs = new BindableProperty<bool>() { Value = false };
        private BindableProperty<bool> destroyingObjs = new BindableProperty<bool>() { Value = false };


        IEnumerator DestoryObjectsInQueue()
        {
            while (destroyedObjectInQueue.Count > 0)
            {
                for (int i = 0; i < NumObjDestroyPerFrame; i++)
                {
                    if (destroyedObjectInQueue.Count > 0)
                    {
                        GameObject obj = destroyedObjectInQueue.Dequeue().gameObject;
                        Destroy(obj);
                    }
                }

                yield return null;
            }

            destroyingObjs.Value = false;
        }

        IEnumerator InitializeObjectsToGame()
        {
            while (numHiddenObjectCreating > 0)
            {
                for (int i = 0; i < NumObjInitPerFrame; i++)
                {
                    if (numHiddenObjectCreating > 0)
                    {
                        GameObject createdObject = base.Allocate();
                        createdObject.SetActive(false);
                        createdObject.transform.SetParent(this.transform);
                        cachedStack.Push(createdObject);
                        numHiddenObjectCreating--;
                    }
                }

                yield return null;
            }

            creatingObjs.Value = false;
            poolState = GameObjectPoolState.Inited;
        }

        private void CheckInited()
        {
            if (poolState == GameObjectPoolState.NotInited)
            {
                Debug.LogError("The SafeGameObject Pool hasn't been inited yet. Use Init() before" +
                               "calling any functions");
            }
        }
    }
}
