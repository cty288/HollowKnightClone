using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MikroFramework.Pool;
using NHibernate.Criterion.Lambda;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class SafeObjectPoolExample : MonoBehaviour
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/Pools/01. SafeObjectPoolExample", false, 1)]
        private static void MenuClicked() {
            

            SafeObjectPool<TestObject>.Singleton.Init(0,30);

            List<TestObject> allocatedObjs = new List<TestObject>();

            for (int i = 0; i < 50; i++) {
                TestObject obj = TestObject.Allocate(i);
                Debug.Log($"Allocated Object ID: {obj.Id}");
                allocatedObjs.Add(obj);
            }

            //Thread.Sleep(1000);

            foreach (TestObject allocatedObj in allocatedObjs) {
                allocatedObj.RecycleToCache();
            }

            TestObject.Allocate(100);
            Debug.Log(SafeObjectPool<TestObject>.Singleton.CurrentCount);
            
        }
#endif


        private class TestObject:IPoolable {
            public int Id;
            public void OnRecycled() {
                Debug.Log("23333");
            }

            public bool IsRecycled { get; set; }

            public void RecycleToCache() {
                SafeObjectPool<TestObject>.Singleton.Recycle(this);
            }

            public static TestObject Allocate(int id) {
                TestObject obj = SafeObjectPool<TestObject>.Singleton.Allocate();
                obj.Id = id;
                return obj;
            }

        }
    }
}
