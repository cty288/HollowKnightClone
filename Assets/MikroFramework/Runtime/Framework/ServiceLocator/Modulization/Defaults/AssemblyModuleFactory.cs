using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    /// <summary>
    /// A default assembly module factory. Elements in this module must inherit a baseModuleType (like an interface).
    /// This factory ONLY create module elements by type
    /// </summary>
    public class AssemblyModuleFactory : IModuleFactory {
        private List<Type> concreteTypeCache;
        /// <summary>
        /// The abstract interface (if exists) for each concrete type cache
        /// </summary>
        private Dictionary<Type, Type> abstractToConcrete = new Dictionary<Type, Type>();


        public AssemblyModuleFactory(Assembly assembly, Type baseModuleType) {
            concreteTypeCache = assembly.GetTypes().Where(t => baseModuleType.IsAssignableFrom(t) && !t.IsAbstract)
                .ToList();

            foreach (Type type in concreteTypeCache) {
                Type[] interfaces = type.GetInterfaces();

                foreach (Type @interface in interfaces) {
                    if (baseModuleType.IsAssignableFrom(@interface) && baseModuleType != @interface) {
                        abstractToConcrete.Add(@interface,type);
                    }
                }
            }
        }

        public object CreateModule(ModuleSearchKeys keys) {
            if (keys.Type.IsAbstract) { //requesting an interface
                if (abstractToConcrete.ContainsKey(keys.Type)) {
                    return abstractToConcrete[keys.Type].GetConstructors().First().Invoke(null);
                }
            }
            else {  //requesting a concrete type
                if (concreteTypeCache.Contains(keys.Type)) {
                    return keys.Type.GetConstructors().First().Invoke(null);
                }

            }
            return null;
        }

        /// <summary>
        /// Returns all modules of Type concreteTypeCache
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public object CreateAllModules() {
            return concreteTypeCache.Select(t => t.GetConstructors().First().Invoke(null));
        }
    }
}
