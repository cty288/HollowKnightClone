using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using MikroFramework.Architecture;
using UnityEngine;

namespace MikroFramework
{
    public class RepeatExample:MonoBehaviour {
        private void Start() {
            /*
            RepeatSequence action = RepeatSequence.Allocate(5);


            action.AddAction(DelayAction.Allocate(1)).AddAction(CallbackAction.Allocate(() => {
                Debug.Log("Delayed 1 seconds");
            })).AddAction(DelayAction.Allocate(1))
                .AddAction(UntilAction.Allocate(()=>Input.GetKeyDown(KeyCode.Space)))
                .AddAction(CallbackAction.Allocate(()=>{Debug.Log("Space pressed");}))
                .Execute();*/

            this.Repeat(5).Delay(2).Event(()=>Debug.Log("delayed 2 sec")).Until(()=>
                Input.GetKeyDown(KeyCode.A)).Event(()=>Debug.Log("Pressed")).Execute();
            
        }
    }
}
