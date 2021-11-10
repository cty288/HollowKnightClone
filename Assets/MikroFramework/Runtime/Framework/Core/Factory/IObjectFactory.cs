using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Factory
{
    public interface IObjectFactory<T>
    {
        /// <summary>
        /// Create an object from the Object's Factory
        /// </summary>
        /// <returns>created object</returns>
        T Create();
    }

}
