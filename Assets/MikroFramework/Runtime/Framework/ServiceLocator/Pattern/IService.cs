using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public interface IService { 
        string Name { get; }
        void Execute();
    }
}
