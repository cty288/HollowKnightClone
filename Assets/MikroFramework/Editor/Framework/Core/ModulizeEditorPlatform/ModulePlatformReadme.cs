using System.Collections;
using System.Collections.Generic;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.EditorModulization {
    public class ModulePlatformReadme :EditorWindow, IEditorPlatformModule {
        public EditorPlatformElement ElementInfo { get; } =
            new EditorPlatformElement(0, "Editor Modulization - Instructions");

        public void OnGUI() {
            GUILayout.Label("Editor Modulization - Instructions", new GUIStyle() {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            });

            GUILayout.Label("1. Create a new class in Editor folder");
            GUILayout.Label("2. Implement IEditorPlatformModule interface like this: ");
            GUILayout.Label("public class ModulePlatformReadme : IEditorPlatformModule{");
            GUILayout.Label("    public void OnGUI() {");
            GUILayout.Label("            //Your code here");
            GUILayout.Label("    }");
            GUILayout.Label("}");

          
        }
    }

}
