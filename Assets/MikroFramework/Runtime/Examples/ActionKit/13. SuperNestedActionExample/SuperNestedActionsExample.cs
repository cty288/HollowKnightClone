using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework
{
    public class SuperNestedActionsExample : MonoBehaviour {
        public float timer;
        private void Start() {
            Timeline timeline = Timeline.Allocate();
            timeline.AddAction(2f, DelayAction.Allocate(1f, () => {
                Debug.Log("Timeline 2s, delayed at 3s");
            }));
            timeline.SetAutoRecycle(false);

            RepeatSequence repeatSequence=RepeatSequence.Allocate(3);
            repeatSequence.AddAction(DelayAction.Allocate(1, ()=>Debug.Log(
                "RepeatSequence delayed 1s")));
            repeatSequence.SetAutoRecycle(false);

            this.Repeat(2)
                .AddAction(timeline).
                AddAction(repeatSequence).
                Delay(2f)
                .Event(() => { Debug.Log("Sequence called at 7s"); })
                //.ResetSelf() //reset every actions above
                .Until(()=>Input.GetKeyDown(KeyCode.Space))
                .Event(()=>Debug.Log("Space down"))
                .Execute();
            

        }

        private void Update() {
            timer += Time.deltaTime;
        }
    }
}
