using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Extensions;
using MikroFramework.Utilities;
using UnityEngine;

namespace MikroFramework.Examples {
    public class DelayWithCoroutineExample : MikroBehavior
    {
        protected override void OnBeforeDestroy()
        {

        }
#if UNITY_EDITOR
        [UnityEditor.MenuItem("MikroFramework/Examples/MikroBehavior/02. Delay Function", false, 2)]
        private static void MenuClicked()
        {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject().AddComponent<DelayWithCoroutineExample>();
        }
#endif

        void Start()
        {
            Delay(5.0f, () => {
                Debug.Log("Time up");
            });
            this. Hide();
        }
    }

}
