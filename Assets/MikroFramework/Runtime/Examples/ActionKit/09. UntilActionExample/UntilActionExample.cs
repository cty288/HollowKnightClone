using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework
{
    public class UntilActionExample : MonoBehaviour {
        

        private void Start() {
            
            UntilAction untilAction=UntilAction.Allocate(()=> {
                return Input.GetKeyDown(KeyCode.Space);
            });
            
            untilAction.OnEndedCallback = () => { Debug.Log("Space down"); };

            untilAction.Execute();



        }


        private bool Test() {
            if (Input.GetKeyDown(KeyCode.A)) {
                return true;
            }

            return false;
        }
    }
}
