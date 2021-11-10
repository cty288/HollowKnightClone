using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MikroFramework
{
    public class UniRxReactiveProperty : MonoBehaviour {
        private ReactiveProperty<int> age = new ReactiveProperty<int>();

        private void Start() {
            //ReactiveProperty will send the initial value (0). If you don't want to receive it, use Skip(1) to skip
            //1 value from the beginning
            age.Skip(1).Subscribe(age => { Debug.Log(age); });

            age.Value = 10;
            age.Value = 10;
            age.Value = 10;
            age.Value = 11;
        }
    }
}
