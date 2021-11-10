using System.Collections;
using System.Collections.Generic;
using MikroFramework.Utilities;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class SimpleRCExample : MonoBehaviour {
        class Light {
            public void Open()
            {
                Debug.Log("Light on");
            }
            public void Close()
            {
                Debug.Log("Light off");
            }
        }

        class Room:SimpleRC {
            private Light light = new Light();
            
            public void EnterPeople() {
                if (RefCount==0) {
                    light.Open();
                }

                Retain();
                Debug.Log($"A person enters the room. The room has {RefCount} people");
            }

            public void LeavePeople() {
                Release();
                Debug.Log($"A person left the room. The room has {RefCount} people");
            }

            protected override void OnZeroRef() {
                light.Close();
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/Utilities/SimpleRCExample", false)]
        private static void MenuItem() {
            Room room = new Room();
            room.EnterPeople();
            room.EnterPeople();
            room.LeavePeople();
            room.LeavePeople();
            room.EnterPeople();
            room.LeavePeople();
        }
#endif
    }


}
