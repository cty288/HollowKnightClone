using System.Collections;
using System.Collections.Generic;
using MikroFramework.ServiceLocator;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public abstract class AbstractModuleLayer : IModuleLayer {
        private ModuleContainer container = null;

        protected abstract AssemblyModuleFactory factory { get; }

        public AbstractModuleLayer() {
            container = new ModuleContainer(new DefaultModuleCache(), factory);
        }
        public T GetModule<T>() where T : class {
            return container.GetModule<T>();
        }
    }
}
