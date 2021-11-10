using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class InterfaceStructExample : MonoBehaviour {
        public interface ICustomScript {
            void Start();
            void Update();
            void Destroy();
        }

        public abstract class CustomScript : ICustomScript {
           void ICustomScript.Start() {
                OnStart();
            }

            void ICustomScript.Update() {
               OnUpdate();
            }

            void ICustomScript.Destroy() {
                OnDestroy();
            }

            protected abstract void OnStart();
            protected abstract void OnUpdate();
            protected abstract void OnDestroy();

        }

        public class MyScript : CustomScript {
            protected override void OnStart() {
                Debug.Log("On Start");
            }

            protected override void OnUpdate() {
                Debug.Log("On Update");
            }

            protected override void OnDestroy() {
                Debug.Log("On Destroy");
            }
        }

        private void Start() {
            ICustomScript myScript = new MyScript();
            myScript.Start();
            myScript.Update();
            myScript.Destroy();
        }
    }
}
