using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework
{
    public class SimplifiedChainActionsExample : MonoBehaviour
    {
        void Start() {
            
            //sequence node
            this.Sequence().Delay(2f).
                Event(() => { Debug.Log("Delayed 2 seconds"); })
                .Until(()=> { return Input.GetKeyDown(KeyCode.Space); }) //the following will not run until press space
                .Delay(3f).
                Event(()=> {Debug.Log("Delayed 3 seconds");}).
                Execute();

            
            //Spawn node (all actions will run simultaneously)
            this.Spawn().
                Delay(3f).
                Event(() => { Debug.Log("This will not run after delay in Spawn"); })
                .Until(() => Input.GetKeyDown(KeyCode.Space), ()=>{Debug.Log("Space pressed");}) //the following will still run even not press space
                .Event(() => { Debug.Log("Another message"); }).Execute();

        }

       
    }
}
