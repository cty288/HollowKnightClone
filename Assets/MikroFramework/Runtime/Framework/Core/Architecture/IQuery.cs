using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework
{
    public interface IQuery<T> : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel,
        ICanGetSystem, ICanGetUtility {
        T Do();
    }
}
