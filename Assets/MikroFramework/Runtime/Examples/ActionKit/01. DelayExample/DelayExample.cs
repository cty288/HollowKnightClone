using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class DelayExample : MonoBehaviour
    {
        private void Awake() {
            Debug.Log(Time.time);
            this.Execute(DelayAction.Allocate(2.0f, () => {
                Debug.Log(Time.time);
            }));


            //another way
            this.Delay(2.0f, () => {
                Debug.Log(Time.time);
            });


            this.Delay(2.0f, () => {
                Debug.Log("2333");
            });
            
        }
    }
}
