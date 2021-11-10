using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class UniRxTimerExample : MonoBehaviour {
        private void Start() {
            Debug.Log("Current: "+DateTime.Now);

            Observable.Timer(TimeSpan.FromSeconds(2.0f)).Subscribe(_ => {
                Debug.Log("Current: " + DateTime.Now);
            }).AddTo(this);

            
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => {
                Destroy(gameObject);
            });
        }
    }
}
