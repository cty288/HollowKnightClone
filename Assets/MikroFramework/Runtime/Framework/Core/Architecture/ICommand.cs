using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public interface ICommand : IBelongToArchitecture,ICanSetArchitecture, ICanGetSystem, ICanGetModel,
    ICanGetUtility, ICanSendEvent, ICanSendCommand, IPoolable, ICanSendQuery{
        void Execute();
    }
}
