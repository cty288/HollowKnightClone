using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public interface IModuleCache {
        /// <summary>
        /// Get a module from search keys
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        object GetModule(ModuleSearchKeys keys);

        /// <summary>
        /// Get all modules
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        object GetAllModules();

        /// <summary>
        /// Add a module, given the module and its search keys
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="module"></param>
        void AddModule(ModuleSearchKeys keys, object module);

    }
}
