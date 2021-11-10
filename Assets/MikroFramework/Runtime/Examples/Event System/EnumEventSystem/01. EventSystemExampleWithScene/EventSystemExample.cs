using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

namespace MikroFramework.Examples
{
    public class EventSystemExample : MikroBehavior {
        protected override void OnBeforeDestroy() {
            
        }

        // Start is called before the first frame update
         void Start() {
            Debug.Log("start");
            AddListener(EventType.Test,HandleOnTest);
            AddListener(EventType.Test1, HandleOnTest);

            Delay(3f, () => {
                Broadcast(EventType.Test,MikroMessage.Create("test message"));

                Delay(2f, () => {
                    RemoveListener(EventType.Test,HandleOnTest);
                    Broadcast(EventType.Test1,MikroMessage.Create(23333));
                });
            });
        }

        private void HandleOnTest(Event.MikroMessage obj)
        {
            Debug.Log(obj.GetSingleMessage());
        }
    }
}
