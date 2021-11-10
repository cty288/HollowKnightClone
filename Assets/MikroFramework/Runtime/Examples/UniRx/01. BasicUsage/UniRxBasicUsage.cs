using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class UniRxBasicUsage : MonoBehaviour
    {
        private void Start() {
            
            Observable.EveryUpdate()
                .Where(_ => Time.frameCount % 5 == 0)
                .Subscribe(_ => Debug.Log("Hello"));


            //var observable = from frame in Observable.EveryUpdate() where Time.frameCount % 5 == 0 select frame;
            //observable.Subscribe(_ => Debug.Log("Hello"));

            IObservable<long> observable = Observable.EveryUpdate()
                .Where(_ => Time.frameCount % 1000 == 0);

            observable.Subscribe(_ => Debug.Log("Hello2"));
        }
    }
}
