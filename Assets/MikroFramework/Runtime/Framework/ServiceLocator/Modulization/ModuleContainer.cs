using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.SqlCommand;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public class ModuleContainer {
        private IModuleCache cache;
        private IModuleFactory factory;

        public ModuleContainer(IModuleCache cache, IModuleFactory factory) {
            this.cache = cache;
            this.factory = factory;
        }

        /// <summary>
        /// Get or Create a module by its type from the container, return the module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetModule<T>() where T : class {
            ModuleSearchKeys moduleSearchKeys = ModuleSearchKeys.Allocate(typeof(T), null);
            object module = cache.GetModule(moduleSearchKeys);

            if (module == null) {
                module = factory.CreateModule(moduleSearchKeys);
                cache.AddModule(moduleSearchKeys,module);
            }

            moduleSearchKeys.RecycleToCache();
            return module as T;
        }


        /// <summary>
        /// Get or create all modules by its type from the container, return those modules
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllModules<T>() where T : class {
            ModuleSearchKeys moduleSearchKeys =ModuleSearchKeys.Allocate(typeof(T),null);

            var modules = cache.GetAllModules() as IEnumerable<object>;

            if (modules == null || !modules.Any()) {
                modules = factory.CreateAllModules() as IEnumerable<object>;

                foreach (object module in modules) {
                    cache.AddModule(moduleSearchKeys,module);
                }
            }
            moduleSearchKeys.RecycleToCache();

            return modules.Select(m => m as T);
        }



    }
}
