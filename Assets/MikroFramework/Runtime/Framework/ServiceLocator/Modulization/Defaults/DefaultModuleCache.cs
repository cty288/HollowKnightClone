using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public class DefaultModuleCache : IModuleCache {

        private Dictionary<Type, List<object>> modulesByType = new Dictionary<Type, List<object>>();
        private Dictionary<string, List<object>> modulesByName = new Dictionary<string, List<object>>();


        public object GetModule(ModuleSearchKeys keys) {
            List<object> output = null;
            if (modulesByType.TryGetValue(keys.Type, out output)) {
                return output.FirstOrDefault();
            }

            return null;
        }

        public object GetAllModules() {
            return modulesByType.Values.SelectMany(list=>list);
        }

        public void AddModule(ModuleSearchKeys keys, object module) {
            if (modulesByType.ContainsKey(keys.Type)) {
                modulesByType[keys.Type].Add(module);
            }
            else {
                modulesByType.Add(keys.Type,new List<object>(){module});
            }
        }

       
    }
}
