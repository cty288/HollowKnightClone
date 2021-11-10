using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public abstract class AbstractInitialContext {
        /// <summary>
        /// Try to find a service
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract IService LookUp(string name);

    }
}
