#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using MikroFramework.EditorModulization;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.ResKit
{
    public class ResKitFrameworkConfig:IEditorPlatformModule {
        public EditorPlatformElement ElementInfo { get; } = new EditorPlatformElement(2, "ResKit");
        public void OnGUI() {
            
            EditorGUILayout.BeginHorizontal();
            ABAssetDataPathConfig();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            HotUpdateFolderConfig();
            EditorGUILayout.EndHorizontal();

        }


        void ABAssetDataPathConfig() {
            string folderPath = EditorPrefs.GetString("ABDataPath", Application.dataPath + "/AssetBundleBuilds/");
            GUILayout.Label(new GUIContent()
            {
                text = "  AB Asset Data Path",
                tooltip = "The path where your Asset Bundles are built when you click \"Build Asset Bundles to Asset Data Path\""
            }, new GUIStyle()
            {
                alignment = TextAnchor.LowerLeft,
                fontSize = 12,
                fixedWidth = 150,
            });

            GUILayout.TextField(folderPath, GUILayout.MaxWidth(700));

            if (GUILayout.Button("Select Path"))
            {

                string searchPath = EditorUtility.OpenFolderPanel("select path", folderPath, "")+"/";

                if (searchPath != "")
                {
                    EditorPrefs.SetString("ABDataPath", searchPath);
                    searchPath = searchPath.Replace(Application.dataPath, "Assets");

                }
            }
        }

        void HotUpdateFolderConfig() {
            string folderPath = PlayerPrefs.GetString("HotupdateFolder", Application.persistentDataPath + "/AssetBundles/");
            
            GUILayout.Label(new GUIContent()
            {
                text = "  Hot Update Folder",
                tooltip = "The path where files are downloaded during hot updating. The default value is persistentDataPath (recommended).\n" +
                          "Note: Changing this will only change the hot-update folder for Unity development, but not the hot-update folder for players when releasing the game. "
            }, new GUIStyle()
            {
                alignment = TextAnchor.LowerLeft,
                fontSize = 12,
                fixedWidth = 150,
            });

            GUILayout.TextField(folderPath, GUILayout.MaxWidth(700));

            if (GUILayout.Button("Select Path"))
            {

                string searchPath = EditorUtility.OpenFolderPanel("select path", folderPath, "")+"/";

                if (searchPath != "")
                {
                    PlayerPrefs.SetString("HotupdateFolder", searchPath);
                }
            }
        }
    }
}
#endif