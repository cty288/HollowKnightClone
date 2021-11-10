using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IEventManager : IModule
    {
        void DoSomething();
    }
    public class EventManager:IEventManager
    {
        private IPoolManager PoolManager { get; set; }
        public void DoSomething() {
            Debug.Log("EventManager DoSomething");
        }

        public void InitModule() {
            PoolManager = ModuleManagementConfig.container.GetModule<IPoolManager>();
        }
    }
}
