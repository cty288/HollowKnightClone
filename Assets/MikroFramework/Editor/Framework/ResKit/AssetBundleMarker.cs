#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MikroFramework.ResKit;
using MikroFramework.Singletons;
using UnityEditor;
using UnityEngine;


namespace MikroFramework.Playground
{

    public class AssetBundleMarker : EditorWindow
    {

        private GUIStyle uiStyle = new GUIStyle("Box");

        private Vector2 filesScrollValue;

        static List<stru_FileInfo> list_Files;

        string assetBundleName;
        string assetBundleVariant = "";

        struct stru_FileInfo
        {
            public string fileName;
            public string filePath;
            public string assetPath;
            public Type assetType;
        }

        [MenuItem("Assets/MikroFramework/ResKit/Set AssetBundle for this Folder")]
        private static void OpenSetAssetBundleNameWindow()
        {
            list_Files = new List<stru_FileInfo>();
            //indentation = 1;
            CheckFileSystemInfo();
            AssetBundleMarker ABNameWin = GetWindow<AssetBundleMarker>("Set AB Name");
            ABNameWin.position = new Rect(300, 100, 300, 500);
            ABNameWin.minSize = new Vector2(300, 500);
            ABNameWin.Show();
        }
        private void OnGUI()
        {
            GUI.skin.label.fontSize = 10;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            FilesGUI();
            SetABNameGUI();
        }
        void FilesGUI()
        {
            //标题
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(150);
            if (GUILayout.Button("Refresh", uiStyle))
            {
                list_Files.Clear();
                CheckFileSystemInfo();
            }
            EditorGUILayout.EndHorizontal();

            GUI.Label(new Rect(5, 35, 100, 20), "Selected " + list_Files.Count + " assets：");
            GUILayout.Space(10);

            filesScrollValue = EditorGUILayout.BeginScrollView(filesScrollValue, uiStyle, GUILayout.MaxHeight(300));
            AddFileGUIToScroll();
            EditorGUILayout.EndScrollView();

            //GUI.EndGroup();
        }

        void AddFileGUIToScroll()
        {
            foreach (stru_FileInfo file in list_Files)
            {

                GUILayout.BeginVertical();

                GUIContent content = EditorGUIUtility.ObjectContent(null, file.assetType);
                content.text = file.fileName;

                GUILayout.Label(content, GUILayout.Height(20));

                GUILayout.EndVertical();
            }
        }
        void SetABNameGUI()
        {
            EditorGUILayout.BeginVertical();
            //设置包名
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            assetBundleName = EditorGUILayout.TextField("AB Name：", assetBundleName);
            EditorGUILayout.EndHorizontal();
            //设置AB版本
            EditorGUILayout.BeginHorizontal();
            assetBundleVariant = EditorGUILayout.TextField("Ext：", assetBundleVariant);
            EditorGUILayout.EndHorizontal();

            //确定设置
            GUILayout.Space(20);
            if (GUILayout.Button("Confirm"))
            {
                for (int a = 0; a < list_Files.Count; a++)
                {
                    SetBundleName(list_Files[a].assetPath);

                }
            }
            EditorGUILayout.EndVertical();

            //GUI.EndGroup();
        }
        #region Set AB name

        private static void CheckFileSystemInfo()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            UnityEngine.Object obj = Selection.activeObject;
            string path = AssetDatabase.GetAssetPath(obj);

            CoutineCheck(path);
        }

        private static void CoutineCheck(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            FileSystemInfo[] fileSystemInfos = directory.GetFileSystemInfos();

            foreach (var item in fileSystemInfos)
            {
                // Debug.Log(item);
                int idx = item.ToString().LastIndexOf(@"\");
                string name = item.ToString().Substring(idx + 1);

                if (!name.Contains(".meta"))
                {
                    CheckFileOrDirectory(item, path + "/" + name);
                }
            }

        }


        private static void CheckFileOrDirectory(FileSystemInfo fileSystemInfo, string path)
        {
            FileInfo fileInfo = fileSystemInfo as FileInfo;
            if (fileInfo != null)
            {
                stru_FileInfo t_file = new stru_FileInfo();
                t_file.fileName = fileInfo.Name;
                t_file.filePath = fileInfo.FullName;
                t_file.assetPath = "Assets" + fileInfo.FullName.Replace(Application.dataPath.Replace("/", "\\"), "");//用于下一步获得文件类型
                t_file.assetType = AssetDatabase.GetMainAssetTypeAtPath(t_file.assetPath);
                t_file.assetPath = path;
                list_Files.Add(t_file);
            }
            else
            {
                CoutineCheck(path);
            }
        }

        private void SetBundleName(string path)
        {
            var importer = AssetImporter.GetAtPath(path);
            string[] strs = path.Split('.');
            string[] dictors = strs[0].Split('/');
            if (importer != null)
            {
                if (assetBundleVariant != "")
                {
                    importer.assetBundleVariant = assetBundleVariant;
                }
                if (assetBundleName != "")
                {
                    importer.assetBundleName = dictors[dictors.Length - 2] + "/" + assetBundleName;
                }
            }
            else
                Debug.Log("importer是空的");
        }

        #endregion
    }
}
#endif