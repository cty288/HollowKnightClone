using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework
{
    public class ResetExample : MonoBehaviour {
        private DelayAction action;
        private void Start() {
             action = DelayAction.Allocate(3, () => {
                Debug.Log("Delayed 3 seconds");
            });

            action.SetAutoRecycle(false);
            action.Execute();
            StartCoroutine(Reset());
        }

        IEnumerator Reset() {
            yield return new WaitForSeconds(2);
            //ResetAction resetAction=ResetAction.Allocate(action);
            //resetAction.Execute();

            action.Reset();
            Debug.Log("Reset and restarted");
            action.Execute();
        }
    }
}
