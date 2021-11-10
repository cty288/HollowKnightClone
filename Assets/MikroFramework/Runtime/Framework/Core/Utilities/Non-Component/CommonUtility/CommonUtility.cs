using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Utilities {
    public partial class CommonUtility
    {
        /// <summary>
        /// Copy a specific text to the clipboard
        /// </summary>
        /// <param name="text">text to copy</param>
        public static void CopyText(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        /// <summary>
        /// Return if the version version1 is bigger than the version version2
        /// </summary>
        /// <param name="newVersion"></param>
        /// <param name="oldVersion"></param>
        /// <returns></returns>
        public static bool CompareVersions(string version1, string version2)
        {
            Version v1 = new Version(version1);
            Version v2 = new Version(version2);
            return v1 > v2;
        }

        public static bool AreVersionsEqual(string version1, string version2) {
            Version v1 = new Version(version1);
            Version v2 = new Version(version2);
            return v1 == v2;
        }

        /// <summary>
        /// Delete the "(Clone)" text on object's name if the object is cloned
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DeleteCloneName(GameObject obj) {
            int cloneNameStartIndex = obj.name.IndexOf("(Clone)");
            if (cloneNameStartIndex >= 0)
            {
                return obj.name.Substring(0, cloneNameStartIndex);
            }

            return obj.name;
        }
    }
}
