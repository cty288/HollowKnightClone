using System.Collections;
using System.Collections.Generic;
using MikroFramework.ServiceLocator;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface ILogicController { }

    public class LogicLayer : AbstractModuleLayer,ILogicLayer
    {
        protected override AssemblyModuleFactory factory {
            get {
                return new AssemblyModuleFactory(typeof(ILogicController).Assembly, typeof(ILogicController));
            }
        }
    }
}
