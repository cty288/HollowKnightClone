using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public interface IModuleLayer {
        T GetModule<T>() where T : class;
    }

    public interface ILogicLayer : IModuleLayer {

    }

    public interface IBusinessModuleLayer : IModuleLayer {

    }

    public interface IPublicModuleLayer : IModuleLayer {

    }

    public interface IBasicModuleLayer : IModuleLayer {

    }

    public interface IUtilityLayer : IModuleLayer {

    }
}
