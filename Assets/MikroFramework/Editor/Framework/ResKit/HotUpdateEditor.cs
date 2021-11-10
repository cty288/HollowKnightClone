/*
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEditor;
using UnityEngine;

namespace MikroFramework.ResKit
{
    public class HotUpdateEditor : EditorWindow
    {
        [MenuItem("MikroFramework/Framework/ResKit/Build HotUpdate Package")]
        private static void Init() {
            HotUpdateEditor window = (HotUpdateEditor) EditorWindow.GetWindow(typeof(HotUpdateEditor), false,
                "Hot Update Panel", true);
        }

        private string resVersionPath = "";
        private string hotUpdateCount = "1";
        private OpenFileEditor openFileEditor = null;

        private void OnGUI() {
            GUILayout.BeginHorizontal();
            resVersionPath = EditorGUILayout.TextField("ResVersion Json File Path: ", resVersionPath,
                GUILayout.Width(350),GUILayout.Height(20));

            if (GUILayout.Button("Choose a ResVersion Json File", GUILayout.Width(250),
                GUILayout.Height(20))) {
                openFileEditor = new OpenFileEditor();
                openFileEditor.structSize = Marshal.SizeOf(openFileEditor);
                openFileEditor.filter = "ResVersion Json File (*.json)\0*.json";
                openFileEditor.file = new string(new char[256]);
                openFileEditor.maxFile = openFileEditor.file.Length;
                openFileEditor.fileTitle = new string(new char[64]);
                openFileEditor.maxFileTitle = openFileEditor.fileTitle.Length;
                openFileEditor.initialDir = HotUpdateConfig.AssetBundleAssetDataBuildPath.Replace("/","\\");
                openFileEditor.title = "Choose ResVersion Json File Window";
                openFileEditor.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
                if (LocalDialog.GetSaveFileName(openFileEditor)) {
                    Debug.Log(openFileEditor.file);
                    resVersionPath = openFileEditor.file;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            hotUpdateCount = EditorGUILayout.TextField("Hot Update Count: ", hotUpdateCount,
                GUILayout.Width(350), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Start Building HotUpdate Package", GUILayout.Width(300),
                GUILayout.Height(50))) {
                if (!string.IsNullOrEmpty(resVersionPath) && resVersionPath.EndsWith(".json")) {
                    AssetBundleExporter.BuildAB(true,resVersionPath,hotUpdateCount);
                }
            }
        }
    }
}
#endif
*/