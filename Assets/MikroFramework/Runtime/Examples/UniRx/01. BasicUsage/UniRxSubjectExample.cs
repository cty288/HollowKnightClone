using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MikroFramework
{
    public class UniRxSubjectExample : MonoBehaviour
    {
        private void Start() {
            Subject<int> subject = new Subject<int>();

            subject.Subscribe(num => {
                Debug.Log(num);
            });


            subject.OnNext(1);
            subject.OnNext(2);
            subject.OnNext(3);
        }
    }
}
