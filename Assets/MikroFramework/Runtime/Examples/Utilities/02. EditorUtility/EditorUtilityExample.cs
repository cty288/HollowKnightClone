#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;
using EditorUtility = MikroFramework.Utilities.EditorUtility;

namespace MyNamespace {
    public class EditorUtilityExample
    {
        [MenuItem("MikroFramework/Examples/Utilities/EditorUtility/1. Call Menu Item Example", false, 1)]
        private static void MenuCallMenuItem()
        {
            EditorUtility.CallMenuItem("GameObject/Create Empty");
        }

    }
}

#endif
