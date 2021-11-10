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
    public class GameObjectUtilityExample
    {
        [MenuItem("MikroFramework/Examples/Utilities/GameObjectExtension/1. Hide an object", false, 1)]
        private static void MenuHideObject()
        {
            GameObject gameObject = new GameObject();

            gameObject.Hide();
        }

    }
}
#endif