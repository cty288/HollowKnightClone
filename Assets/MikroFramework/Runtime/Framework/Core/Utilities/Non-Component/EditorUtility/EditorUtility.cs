using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MikroFramework.Utilities
{
    public partial class EditorUtility
    {
        /// <summary>
        /// Open a specific folder
        /// </summary>
        /// <param name="folderPath">path of the folder</param>
        public static void OpenInFolder(string folderPath)
        {
            Application.OpenURL("file:///" + folderPath);
        }
#if UNITY_EDITOR
        /// <summary>
        /// Export this framework as unitypackage.
        /// </summary>
        /// <param name="assetPathName"></param>
        /// <param name="fileName"></param>
        public static void ExportPackage(string assetPathName, string fileName)
        {
            AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
        }
        public static void CallMenuItem(string menuName)
        {
            EditorApplication.ExecuteMenuItem(menuName);
        }
#endif
    }
}
