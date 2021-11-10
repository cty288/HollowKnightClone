using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples.ServiceLocator
{
    public interface IFSM : IModule
    {
        void DoSomething();
    }
    public class FSM:IFSM {
        public void DoSomething() {
            Debug.Log("FSM Dosomething");
        }

        public void InitModule() {
            
        }
    }
}
