#if UNITY_EDITOR


using System;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.Utilities {
    public class Exporter
    {
        private static string GenerateBuildPackageName() {
            AddCurrentFrameworkBuildVersion();
            return "Builds/MikroFramework_v." + GetCurrentFrameworkVersionString() + ".unitypackage";
        }

        private static string GenerateMinorPackageName()
        {
            AddCurrentFrameworkMinorVersion();
            return "Builds/MikroFramework_v." + GetCurrentFrameworkVersionString() + ".unitypackage";
        }

        private static string GenerateMajorPackageName()
        {
            AddCurrentFrameworkMajorVersion();
            return "Builds/MikroFramework_v." + GetCurrentFrameworkVersionString() + ".unitypackage";
        }

        private static void AddCurrentFrameworkBuildVersion() {
            EditorPrefs.SetInt("FrameworkVersion_Build",
                EditorPrefs.GetInt("FrameworkVersion_Build", 26)+1);
        }

        private static void AddCurrentFrameworkMinorVersion() {
            EditorPrefs.SetInt("FrameworkVersion_Minor",
                EditorPrefs.GetInt("FrameworkVersion_Minor", 0) + 1);

            EditorPrefs.SetInt("FrameworkVersion_Build",0);
        }

        private static void AddCurrentFrameworkMajorVersion() {
            EditorPrefs.SetInt("FrameworkVersion_Major",
                EditorPrefs.GetInt("FrameworkVersion_Major", 0) + 1);

            EditorPrefs.SetInt("FrameworkVersion_Minor", 0);

            EditorPrefs.SetInt("FrameworkVersion_Build", 0);

            /*EditorPrefs.SetInt("FrameworkVersion_Major",
                0);

            EditorPrefs.SetInt("FrameworkVersion_Minor", 0);

            EditorPrefs.SetInt("FrameworkVersion_Build", 27);*/
        }

        private static string GetCurrentFrameworkVersionString() {
            return EditorPrefs.GetInt("FrameworkVersion_Major", 0) + "." +
                   EditorPrefs.GetInt("FrameworkVersion_Minor", 0) + "." +
                   EditorPrefs.GetInt("FrameworkVersion_Build", 26);
        }

        [MenuItem("MikroFramework/Framework/Editor/Export a new build version of this framework %e", false, 1)]
        private static void MenuCreateUnityBuildPackage()
        {
            string fileName = GenerateBuildPackageName();
            GenerateFrameworkPackage(fileName);
        }

        [MenuItem("MikroFramework/Framework/Editor/Export a new minor version of this framework %t", false, 2)]
        private static void MenuCreateUnityMinorPackage()
        {
            string fileName = GenerateMinorPackageName();
            GenerateFrameworkPackage(fileName);
        }

        [MenuItem("MikroFramework/Framework/Editor/Export a new major version of this framework %u", false, 3)]
        private static void MenuCreateUnityMajorPackage()
        {
            string fileName = GenerateMajorPackageName();
            GenerateFrameworkPackage(fileName);
        }

        private static void GenerateFrameworkPackage(string fileName) {
            CommonUtility.CopyText(fileName);

            string assetPathName = "Assets/MikroFramework";

            Debug.Log($"Generated file: {fileName}");

            EditorUtility.ExportPackage(assetPathName, fileName);

            EditorUtility.OpenInFolder(Environment.CurrentDirectory + "/Builds");
        }

    }
}

#endif