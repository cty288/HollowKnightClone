using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ServiceLocator;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public class ModuleManagementConfig : MonoBehaviour {
        public static ModuleContainer container = null;

        private void Awake() {
            Type baseType = typeof(IModule);
            AssemblyModuleFactory factory = new AssemblyModuleFactory(baseType.Assembly, baseType);
            DefaultModuleCache cache = new DefaultModuleCache();

            container = new ModuleContainer(cache, factory);

            IPoolManager poolManager = container.GetModule<IPoolManager>();
            IFSM fsm = container.GetModule<IFSM>();
            IResManager resManager = container.GetModule<IResManager>();
            IEventManager eventManager = container.GetModule<IEventManager>();
            IUIManager uiManager = container.GetModule<IUIManager>();

            /*
            (uiManager as UIManager).EventManager = eventManager;
            (uiManager as UIManager).ResManager = resManager;
            (eventManager as EventManager).PoolManager = poolManager;
            (resManager as ResManager).PoolManager = poolManager;
            */

            foreach (var module in container.GetAllModules<IModule>()) {
                module.InitModule();
            }

        }

        private void Start() {
            IUIManager uiManager = ModuleManagementConfig.container.GetModule<IUIManager>();
            uiManager.DoSomething();
        }
    }
}
