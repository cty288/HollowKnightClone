using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel, ICanGetUtility,
    ICanSendEvent,ICanRegisterEvent,ICanGetSystem{
        void Init();
    }
}
