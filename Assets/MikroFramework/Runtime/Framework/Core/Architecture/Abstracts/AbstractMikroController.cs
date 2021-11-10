using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MikroFramework.Architecture
{
    public abstract class AbstractMikroController<T> : MonoBehaviour, IController where T:Architecture<T>,new()
    {
        IArchitecture IBelongToArchitecture.GetArchitecture() {
            return Architecture<T>.Interface;
        }
    }
}
