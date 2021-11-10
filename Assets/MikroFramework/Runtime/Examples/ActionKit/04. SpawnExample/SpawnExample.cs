using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MikroFramework.ActionKit;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MikroFramework.Examples
{
    public class SpawnExample : MonoBehaviour
    {
       
        void Start()
        {
            
            Spawn spawn = Spawn.Allocate();

            double totalElapsedSeconds = 0;

            for (int i = 0; i < 300; i++)
            {
                spawn.AddAction(CallbackAction.Allocate(() => {
                    Debug.Log($"spawned at "+ DateTime.Now);
                }));
            }

            spawn.AddAction(DelayAction.Allocate(1, () => {
                Debug.Log("delay finished at "+DateTime.Now);
            }));

            spawn.Execute();


        }

        
    }
}
