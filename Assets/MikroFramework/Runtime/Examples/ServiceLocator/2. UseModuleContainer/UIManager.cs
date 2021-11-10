using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IUIManager : IModule
    {
        void DoSomething();
    }
    public class UIManager:IUIManager
    {
        private IResManager ResManager { get; set; }
        private IEventManager EventManager { get; set; }

        public void DoSomething() {
            Debug.Log("UIManager DoSomething");

            ResManager.DoSomething();
            EventManager.DoSomething();
        }

        public void InitModule() {
            ResManager = ModuleManagementConfig.container.GetModule<IResManager>();
            EventManager = ModuleManagementConfig.container.GetModule<IEventManager>();
        }
    }
}
