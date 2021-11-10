#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class MathUtilityExample : MonoBehaviour
    {
        [MenuItem("MikroFramework/Examples/Utilities/MathUtility/01. 50% Random Possibility", false, 1)]
        private static void MenuClicked()
        {
            Debug.Log(MathUtility.Percent(50));
        }

        [MenuItem("MikroFramework/Examples/Utilities/MathUtility/02. Get a Random Element from an Array", false, 2)]
        private static void MenuRandomElementClicked()
        {
            Debug.Log(MathUtility.GetRandomValueFrom(1, 5, 2, 5, 10));
        }

    }
}
#endif