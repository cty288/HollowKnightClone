using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework
{
    public class ComplexRepeatExample : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start() {

            
            this.Repeat() //this will run infinitely
                .Delay(1)
                .Event(() => Debug.Log("Delayed 1 second"))
                .Until(() => Input.GetKeyDown(KeyCode.Space))
                .Event(() => Debug.Log("Space pressed"))
                .ResetSelf()//turn it into infinite repeat
                .Event(() => Debug.Log("This will not run"))
                .Execute();


            /*
            this.Sequence()//not infinite; only once
                .Delay(1)
                .Event(()=>Debug.Log("Delayed 1 second"))
                .Until(()=>Input.GetKeyDown(KeyCode.Space))
                .Event(()=>Debug.Log("Space pressed"))
                .ResetSelf()//the following will not run
                .Event(()=>Debug.Log("This will not run")) 
                .Execute();*/

            /*
            this.Spawn()//error
                .Delay(1)
                .Event(() => Debug.Log("Delayed 1 second"))
                .Until(() => Input.GetKeyDown(KeyCode.Space))
                .Event(() => Debug.Log("Space pressed"))
                .ResetSelf()//the following will not run
                .Event(() => Debug.Log("This will not run"))
                .Execute();*/
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
