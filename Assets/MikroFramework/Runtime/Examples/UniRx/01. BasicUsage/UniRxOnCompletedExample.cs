using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MikroFramework
{
    public class UniRxOnCompletedExample : MonoBehaviour {
        private void Start() {
            Observable.Timer(TimeSpan.FromSeconds(3.0f)).Subscribe(_ => {
                Debug.Log("Delayed 3 seconds");
            }, () => Debug.Log("completed"));
        }
    }
}
