using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MikroFramework.IOC
{
    public class IOCContainer:IIOCContainer {
        private HashSet<Type> registeredType = new HashSet<Type>();

        private Dictionary<Type, object> instances = new Dictionary<Type, object>();

        private Dictionary<Type, Type> dependency = new Dictionary<Type, Type>();

        /// <summary>
        /// RegisterInstance a singleton to the IOC Container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void RegisterInstance<T>(object instance) {
            Type key = typeof(T);

            if (!instances.ContainsKey(key)) {
                instances.Add(key,instance);
            }
            else {
                instances[key] = instance;
            }
        }

        public void RegisterInstance(object instance) {
            Type type = instance.GetType();
            if (!instances.ContainsKey(type)) {
                instances.Add(type, instance);
            }else {
                instances[type] = instance;
            }

        }


        public void RegisterInstance<TBase, TConcrete>() where TConcrete : TBase
        {
            Type concreteObj = typeof(TConcrete);
            Type baseObj = typeof(TBase);

            if (!dependency.ContainsKey(baseObj))
            {
                dependency.Add(baseObj, concreteObj);
            }
            else
            {
                dependency[baseObj] = concreteObj;
            }
        }

        public void Register<T>() {
             registeredType.Add(typeof(T));
        }

       
        


        public void Inject(object obj) {
            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties().Where(p =>
                p.GetCustomAttributes(typeof(IOCInject)).Any()))
            {

                var instance = GetSingletonOrObject(propertyInfo.PropertyType);

                if (instance != null)
                {
                    propertyInfo.SetValue(obj, instance);
                }
                else
                {
                    Debug.LogError($"Can't inject object of type: {propertyInfo.PropertyType}");
                }
            }
        }

        public void Clear() {
            registeredType.Clear();
            dependency.Clear();
            instances.Clear();
        }

       

        public T Get<T>() where T : class {
            Type type = typeof(T);

            if (registeredType.Contains(type)) {
                return Activator.CreateInstance<T>();
            }

            return null;
        }

        public T GetInstance<T>() where T : class {
            Type type = typeof(T);

            if (instances.ContainsKey(type))
            {
                return instances[type] as T;
            }

            if (dependency.ContainsKey(type))
            {
                return Activator.CreateInstance(dependency[type]) as T;
            }

            return null;
        }

        private object GetSingletonOrObject(Type type)
        {
            if (instances.ContainsKey(type))
            {
                return instances[type];
            }

            if (dependency.ContainsKey(type))
            {
                return Activator.CreateInstance(dependency[type]);
            }

            if (registeredType.Contains(type))
            {
                return Activator.CreateInstance(type);
            }

            return default;
        }
    }
}
