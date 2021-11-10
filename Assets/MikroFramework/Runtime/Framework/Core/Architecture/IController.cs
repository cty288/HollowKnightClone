using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public interface IController : IBelongToArchitecture, ICanSendCommand, ICanGetSystem, ICanGetModel, ICanRegisterEvent,
    ICanSendQuery{
        
    }
}
