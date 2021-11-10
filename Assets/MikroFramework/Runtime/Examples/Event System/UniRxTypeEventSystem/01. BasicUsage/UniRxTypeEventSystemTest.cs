using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MikroFramework
{
    public class UniRxTypeEventSystemTest : MonoBehaviour
    {
        class A {

        }

        class B {
            public int Age;
            public string Name;
        }

        private IDisposable eventA;

        private void Start() {
            eventA = UniRxTypeEventSystem.GetEvent<A>().Subscribe(ReceiveA);

            UniRxTypeEventSystem.GetEvent<B>().Subscribe(ReceiveB).AddTo(this.gameObject);

        }

        private void ReceiveB(B obj) {
            Debug.Log($"Received B: {obj.Name}, {obj.Age}");
        }

        private void ReceiveA(A obj) {
            Debug.Log("Received A");
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                UniRxTypeEventSystem.Send(new A());
            }

            if (Input.GetMouseButtonDown(1)) {
                UniRxTypeEventSystem.Send(new B() {
                    Age = Random.Range(0,100),
                    Name = "Test"
                });
            }

            if (Input.GetKeyDown(KeyCode.U)) {
                eventA.Dispose();
            }
        }
    }
}
