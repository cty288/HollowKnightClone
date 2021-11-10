#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.Examples {
    public class ResolutionCheckExample : MonoBehaviour
    {
        [MenuItem("MikroFramework/Examples/Utilities/ResolutionCheck/1. Check Resolution", false, 1)]
        private static void MenuClicked()
        {
            Debug.Log(ResolutionCheck.IsPadResolution4_3() ? "Is Pad" : "Not Pad");
            Debug.Log(ResolutionCheck.IsIPhoneResolution16_9() ? "Is iPhone" : "Not iPhone");
            Debug.Log(ResolutionCheck.IsIPhone4SResolution() ? "Is iPhone4S" : "Not iPhone4S");
            Debug.Log(ResolutionCheck.IsIphoneXResolution() ? "Is iPhoneX" : "Not iPhoneX");
        }

    }
}
#endif

