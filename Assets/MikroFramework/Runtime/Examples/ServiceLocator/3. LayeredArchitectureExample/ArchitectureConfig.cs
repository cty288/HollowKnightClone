using System.Collections;
using System.Collections.Generic;
using MikroFramework.Examples;
using MikroFramework.ServiceLocator;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public class ArchitectureConfig : IArchitecture
    {
        public ILogicLayer LogicLayer { get; private set; }
        public IBusinessModuleLayer BusinessModuleLayer { get; private set; }
        public IPublicModuleLayer PublicModuleLayer { get; private set; }
        public IUtilityLayer UtilityLayer { get; private set; }
        public IBasicModuleLayer BasicModuleLayer { get; private set; }

        public static ArchitectureConfig Architecture = null;

        [RuntimeInitializeOnLoadMethod]
        static void Config() {
            Architecture = new ArchitectureConfig();

            Architecture.LogicLayer = new LogicLayer();

            Architecture.BusinessModuleLayer = new BusinessModuleLayer();

        }
    }
}
