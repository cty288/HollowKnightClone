using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public interface IArchitecture {

       ILogicLayer LogicLayer { get; }
       IBusinessModuleLayer BusinessModuleLayer { get; }
       /// <summary>
       /// Modules that serve as solutions for specific problems. Such as UIManager, ResManager...
       /// </summary>
       IPublicModuleLayer PublicModuleLayer { get; }
       /// <summary>
       /// Tools that do not need to manage data. Such as JsonSerializer
       /// </summary>
       IUtilityLayer UtilityLayer { get; }

       /// <summary>
       /// Basic Modules, such as Log and third-party plugins, such as DOTween and BestHTTP
       /// </summary>
       IBasicModuleLayer BasicModuleLayer { get; }

    }
}
