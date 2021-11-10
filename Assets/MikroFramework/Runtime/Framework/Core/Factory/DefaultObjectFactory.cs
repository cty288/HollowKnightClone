using System.Collections;
using System.Collections.Generic;
using MikroFramework.Factory;
using UnityEngine;

namespace MikroFramework.Factory
{
    /// <summary>
    /// Default Object Factory: Objects are created by new()
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultObjectFactory<T>:IObjectFactory<T> where T:new()
    {
        public T Create() {
            return new T();
        }
    }
}
