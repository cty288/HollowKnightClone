using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IResManager : IModule
    {
        void DoSomething();
    }
    public class ResManager:IResManager 
    {
        private IPoolManager PoolManager { get; set; }
        public void DoSomething() {
            Debug.Log("ResManager DoSomething");
        }

        public void InitModule() {
            PoolManager = ModuleManagementConfig.container.GetModule<IPoolManager>();
        }
    }
}
