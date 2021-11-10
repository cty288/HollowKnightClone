using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Event;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

namespace MikroFramework
{
    public class UseEventCenterExample : MonoBehaviour
    {
        private void Start() {
            EventCenter.AddListener(EventType.Test, (msg) => {
                Debug.Log(msg.GetSingleMessage());
            });

            EventCenter.AddListener(EventType.Test1, (msg) => {
                Debug.Log(msg.GetSingleMessage());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.A)) {
                EventCenter.Broadcast(EventType.Test,MikroMessage.Create("1"));
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                EventCenter.Broadcast(EventType.Test1, MikroMessage.Create("2"));
            }
        }
    }
}
