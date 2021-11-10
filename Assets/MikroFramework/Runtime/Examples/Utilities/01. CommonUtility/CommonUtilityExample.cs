#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.Examples {
    public class CommonUtilityExample
    {
        [MenuItem("MikroFramework/Examples/Utilities/CommonUtility/1. Copy Text to the Clipboard", false, 1)]
        private static void MenuCopyTest()
        {
            CommonUtility.CopyText("Test copy test");
        }

    }
}
#endif

