using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using MikroFramework.ActionKit;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MikroFramework.Examples
{
    public class SequenceExample : MonoBehaviour
    {
        void Start() {
            
            Sequence sequence = Sequence.Allocate();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double totalElapsedSeconds = 0;

            for (int i = 0; i < 300; i++)
            {
                sequence.AddAction(CallbackAction.Allocate(() => {
                    stopwatch.Stop();
                    totalElapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Start();
                }));
            }

            sequence.AddAction(DelayAction.Allocate(1f, () => {
                Debug.Log("Delayed");

                stopwatch.Stop();
                Debug.Log(stopwatch.Elapsed.TotalSeconds);
            }));
            sequence.Execute();




        }
        
    }
}
