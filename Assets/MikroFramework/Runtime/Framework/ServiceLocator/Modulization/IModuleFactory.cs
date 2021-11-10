using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public interface IModuleFactory {
        
        object CreateModule(ModuleSearchKeys keys);
        object CreateAllModules();
    }
}
