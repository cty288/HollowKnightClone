using System.Collections;
using System.Collections.Generic;
using MikroFramework.ServiceLocator;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface ISystem { }
    public class BusinessModuleLayer : AbstractModuleLayer, IBusinessModuleLayer {
        protected override AssemblyModuleFactory factory {
            get {
                return new AssemblyModuleFactory(typeof(ISystem).Assembly, typeof(ISystem));
            }
        }
    }
}
