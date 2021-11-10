using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Factory
{
    public class CustomObjectFactory<T> : IObjectFactory<T>
    {
        private Func<T> factoryCreateMethod;
        /// <summary>
        /// The method that can create or get the Object of type T
        /// </summary>
        /// <param name="factoryMethod"></param>
        public CustomObjectFactory(Func<T> factoryMethod)
        {
            factoryCreateMethod = factoryMethod;
        }
        public T Create()
        {
            return factoryCreateMethod();
        }
    }
}
