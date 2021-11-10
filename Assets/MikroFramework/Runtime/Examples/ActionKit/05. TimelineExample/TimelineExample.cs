using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MikroFramework.ActionKit;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MikroFramework.Examples
{
    public class TimelineExample : MonoBehaviour
    {
        
        void Start() {
            bool callbackCalled = false;


            Timeline timeline = Timeline.Allocate();
            timeline.AddAction(1.0f, CallbackAction.Allocate(() => {
                Debug.Log("Called at 1s");
            }));


            timeline.AddAction(2.0f, CallbackAction.Allocate(() => {
                Debug.Log("Called at 2s");
            }));

            timeline.AddAction(5.0f, CallbackAction.Allocate(() => {
                Debug.Log("Called at 5s");
            }));

            timeline.AddAction(2.0f, CallbackAction.Allocate(() => {
                Debug.Log("Called at 2s again");
            }));

            //timeline.Execute();
            this.Execute(timeline);

            

        }

        
    }
}
