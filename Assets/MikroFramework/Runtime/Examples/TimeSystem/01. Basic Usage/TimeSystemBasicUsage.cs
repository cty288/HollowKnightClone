using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class TimeSystemBasicUsage : MonoBehaviour
    {
        private void Start() {
            TimeSystem.TimeSystem ts = new TimeSystem.TimeSystem();

            ts.AddDelayTask(3, () => { Debug.Log("3 sec"); })
                .AddDelayTask(5, ()=> Debug.Log("5 sec"));

            ts.Start();
            ts.Start();
            ts.Start();

            StartCoroutine(Delay(ts));
        }

        IEnumerator Delay(TimeSystem.TimeSystem ts) {
            yield return new WaitForSeconds(6);
            ts.Reset();
            ts.Pause();
            Debug.Log("reset and pause");
            StartCoroutine(Continue(ts));
        }

        IEnumerator Continue(TimeSystem.TimeSystem ts) {
            yield return new WaitForSeconds(2);
            ts.Start();
            Debug.Log("Resume");
        }
    }
}
