#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Extensions;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;
using EditorUtility = MikroFramework.Utilities.EditorUtility;
namespace MikroFramework.Examples
{
    public class Hide :MikroBehavior
    {
        [UnityEditor.MenuItem("MikroFramework/Examples/MikroBehavior/01. Hide an Object", false, 1)]
        private static void MenuHideClicked()
        {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject().AddComponent<Hide>();
        }


        protected override void OnBeforeDestroy() {
            
        }

        void Start() {
            this.Hide();
        }
    }
}
#endif