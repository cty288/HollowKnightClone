using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikroFramework.IOC
{
    public interface IIOCContainer {
        /// <summary>
        /// RegisterInstance a object (not singleton) of type T to the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Register<T>();

        /// <summary>
        /// RegisterInstance an existing singleton of type T to the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        void RegisterInstance<T>(object instance);


        /// <summary>
        /// RegisterInstance an existing singleton to the container
        /// </summary>
        /// <param name="instance"></param>
        void RegisterInstance(object instance);


        void RegisterInstance<TBase, TConcrete>() where TConcrete : TBase;

        /// <summary>
        /// Get a new instance of a registered object from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T:class;


        /// <summary>
        /// Get a singleton from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetInstance<T>() where T : class;


        /// <summary>
        /// Inject all dependency properties of an object. Those properties should be marked attributes [IOCInject],
        /// and they should be registered into the IOC Container
        /// </summary>
        /// <param name="obj"></param>
        void Inject(object obj);

        void Clear();
    }
}
