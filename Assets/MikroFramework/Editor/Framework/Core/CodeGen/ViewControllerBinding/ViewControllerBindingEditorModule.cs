using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.EditorModulization;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.CodeGen
{
    public class ViewControllerBindingEditorModule:IEditorPlatformModule {
        public EditorPlatformElement ElementInfo { get; } = new EditorPlatformElement(3, "Code Generation");

        public void OnGUI() {
            CreateTextInputWithDescriptionAndButton("DefaultCodeGenRootPath",
                Application.dataPath + "/ViewControllers/",
                "Default View Controller Code Generation Path",
                "The root path where all view controller codes are generated");


            CreateTextInputWithDescription("DefaultCodeGenAssembly", 
                "Assembly-CSharp",
                "Default Code Generation Assembly Name",
                "The default assembly name where the generated scripts are located");


        }


        private void CreateTextInputWithDescription(string inputBoxEditorPrefKey,
            string editorPrefDefaultValue,
            string labelText, string labelTooltip) {


            EditorGUILayout.BeginHorizontal();
            string value = EditorPrefs.GetString(inputBoxEditorPrefKey, editorPrefDefaultValue);
            GUILayout.Label(new GUIContent()
            {
                text = "  " + labelText,
                tooltip = labelTooltip
            }, new GUIStyle()
            {
                alignment = TextAnchor.LowerLeft,
                fontSize = 12,
            });

            GUILayout.TextField(value);

            EditorPrefs.SetString(inputBoxEditorPrefKey, value);

            EditorGUILayout.EndHorizontal();

        }

        private void CreateTextInputWithDescriptionAndButton(string inputBoxEditorPrefKey,
            string editorPrefDefaultValue,
            string labelText, string labelTooltip, string buttonText = "Select Path") {

            EditorGUILayout.BeginHorizontal();
            string folderPath = EditorPrefs.GetString(inputBoxEditorPrefKey, editorPrefDefaultValue);
            GUILayout.Label(new GUIContent()
            {
                text = "  "+labelText,
                tooltip = labelTooltip
            }, new GUIStyle()
            {
                alignment = TextAnchor.LowerLeft,
                fontSize = 12,
            });

            GUILayout.TextField(folderPath);

            if (GUILayout.Button(buttonText))
            {

                string searchPath = EditorUtility.OpenFolderPanel("select path", folderPath, "") + "/";

                if (searchPath != "")
                {
                    EditorPrefs.SetString(inputBoxEditorPrefKey, searchPath);
                    searchPath = searchPath.Replace(Application.dataPath, "Assets");

                }
            }

            EditorGUILayout.EndHorizontal();
        }

    }
}
