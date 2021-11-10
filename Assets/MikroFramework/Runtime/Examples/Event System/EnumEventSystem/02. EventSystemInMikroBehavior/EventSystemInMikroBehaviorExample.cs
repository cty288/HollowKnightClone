using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Event;
using UnityEngine;
using EventType = MikroFramework.Event.EventType;

namespace MikroFramework.Examples {
    public class EventSystemInMikroBehavior : MikroBehavior
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/EventSystem/01. Event System Integrated in MikroBehavior", false, 1)]
        private static void MenuClicked()
        {
            UnityEditor.EditorApplication.isPlaying = true;
            GameObject obj = new GameObject();
            obj.AddComponent<EventSystemInMikroBehavior>();
        }
#endif
        protected override void OnBeforeDestroy()
        {
        }

        void Awake()
        {
            AddListener(EventType.Test, DoSth);
            AddListener(EventType.Test1, DoSth);

            RemoveListener(EventType.Test);
            StartCoroutine(TriggerEvent());
        }

        void DoSth(MikroMessage data)
        {
            foreach (object msg in data.Messages)
            {
                Debug.Log(msg.ToString());
            }
        }

        IEnumerator TriggerEvent()
        {
            yield return new WaitForSeconds(2);
            Broadcast(EventType.Test, MikroMessage.Create("23333"));
            yield return new WaitForSeconds(2);
            Broadcast(EventType.Test1, MikroMessage.Create("233", 555));
        }
    }

}
