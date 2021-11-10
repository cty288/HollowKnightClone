#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using MikroFramework.Extensions;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;


namespace MikroFramework.Examples
{
    public class TransformUtilityExample : MonoBehaviour
    {
        [MenuItem("MikroFramework/Examples/Utilities/TransformExtension/1. Set Position and Reset Transform", false, 1)]
        private static void MenuClicked()
        {
            GameObject gameObject = new GameObject();
            gameObject.transform.SetLocalPosX(5f);
            gameObject.transform.SetLocalPosY(5f);
            gameObject.transform.SetLocalPosZ(5f);

            gameObject.transform.SetLocalPosXY(10,10);
            gameObject.transform.Identity();
        }

        [MenuItem("MikroFramework/Examples/Utilities/TransformExtension/2. Set Parent", false, 2)]
        private static void MenuSetParentClicked()
        {
            GameObject parent = new GameObject("Parent");
            GameObject child = new GameObject("Child");

            parent.transform.AddChild(child.transform);

        }

    }
}
#endif