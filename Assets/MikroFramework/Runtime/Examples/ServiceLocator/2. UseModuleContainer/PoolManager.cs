using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IPoolManager : IModule {
        void DoSomething();
    }
    
    public class PoolManager:IPoolManager {
        public void DoSomething() {
            Debug.Log("PoolManager DoSomething");
        }

        public void InitModule() {
            
        }
    }
}
