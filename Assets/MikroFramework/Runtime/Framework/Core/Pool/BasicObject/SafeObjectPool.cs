using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using MikroFramework.Factory;
using MikroFramework.Singletons;
using UniRx;
using UnityEngine;

namespace MikroFramework.Pool
{
    /// <summary>
    /// A safer Object ObjectPool, which is a singleton pool for the type of poolable object. The number of objects in the pool
    /// will not exceed the MaxCount. Object of the pool must inherit IPoolable interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SafeObjectPool<T> : ObjectPool<T>, ISingleton where T : class, IPoolable, new()
    {
        private int maxCount;
        private ReactiveProperty<int> numActiveObject = new ReactiveProperty<int>(0);

        /// <summary>
        /// Return the number of allocated active object of this pool
        /// </summary>
        public ReactiveProperty<int> NumActiveObject => numActiveObject;

        public int MaxCount
        {
            get
            {
                return maxCount;
            }
            set
            {
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
                        cachedStack.Pop();
                    }
                }
            }
        }

        protected SafeObjectPool()
        {
            factory = new DefaultObjectFactory<T>();
        }

        void ISingleton.OnSingletonInit()
        {
            numActiveObject.Subscribe((newValue) => {
                TypeEventSystem.SendGlobalEvent(new OnPoolValueChanged()
                {
                    PoolName = typeof(T).Name,
                    Value = newValue,
                    MaxValue = MaxCount
                });
            });

            Init();
        }

        public static SafeObjectPool<T> Singleton
        {
            get
            {
                SafeObjectPool<T> instance = SingletonProperty<SafeObjectPool<T>>.Singleton;
                return SingletonProperty<SafeObjectPool<T>>.Singleton;
            }
        }

        public void Init(int initialCount = 0, int maxCount = 50)
        {
            if (initialCount > maxCount)
            {
                Debug.LogError("Initial count of the SafeObjectPool can't be bigger than the maxCount");
            }

            if (initialCount < 0 || maxCount < 0)
            {
                Debug.LogError("Initial count or Max Count must be greater or equal to 0");
            }

            MaxCount = maxCount;

            if (CurrentCount < initialCount)
            {
                for (int i = CurrentCount; i < initialCount; i++)
                {
                    T createdObject = factory.Create();
                    createdObject.IsRecycled = false;
                    cachedStack.Push(createdObject);
                }
            }

        }

        /// <summary>
        /// Allocare an instance from the SafeObjectPool.
        /// </summary>
        /// <returns></returns>
        public override T Allocate()
        {
            T instance = base.Allocate();
            instance.IsRecycled = false;
            numActiveObject.Value++;
            return instance;
        }

        /// <summary>
        /// Recycle the IPoolable object back to the ObjectPool, and trigger its OnRecycled Function.
        /// If the ObjectPool is full, it will not return to the pool
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Recycle(T obj)
        {
            if (obj == null || obj.IsRecycled)
            {
                return false;
            }

            numActiveObject.Value--;
            if (CurrentCount < maxCount)
            {
                obj.IsRecycled = true;
                obj.OnRecycled();
                cachedStack.Push(obj);
                return true;
            }
            else
            {
                Debug.Log($"The SafeObjectPool {typeof(T)} is full! The Object will not return to the pool");
                obj.OnRecycled();
                return false;
            }
        }
    }
}
