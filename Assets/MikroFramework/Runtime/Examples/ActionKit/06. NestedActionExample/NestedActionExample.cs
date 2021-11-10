using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class NestedActionExample : MonoBehaviour
    {
        private void Start() {
            bool called = false;
            ActionKit.ActionKit actionKit = new ActionKit.ActionKit();

            Sequence sequence = Sequence.Allocate();
            Spawn spawn = Spawn.Allocate();
            Timeline timeline = Timeline.Allocate();

            sequence.AddAction(spawn).
                AddAction(DelayAction.Allocate(2f)).
                AddAction(CallbackAction.Allocate(()=>{Debug.Log("Sequence -- delayed 2 seconds -- callback action triggered");}))
                .AddAction(timeline) //this will not run cuz the timeline object has finished
                .AddAction(CallbackAction.Allocate(()=>{Debug.Log("This will run");}));
            
            
            spawn.AddAction(timeline).
                AddAction(CallbackAction.Allocate(()=>{Debug.Log("Spawn Callback");}));

            timeline.AddAction(1.0f, CallbackAction.Allocate(() => { Debug.Log("timeline callback"); }));

            this.Execute(sequence);
           
        }
    }
}
