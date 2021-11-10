using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.ServiceLocator;
using UnityEditor;
using UnityEngine;

namespace MikroFramework.EditorModulization
{

    public struct EditorPlatformElement
    {
        public int Index;
        public string SidebarName;


        public EditorPlatformElement(int index, string sidebarName)
        {
            this.Index = index;
            this.SidebarName = sidebarName;
        }
    }

    public class EditorModulizationPlatformEditor : EditorWindow {
        private ModuleContainer moduleContainer = null;
        private Dictionary<int, string> editorPlatformElements = new Dictionary<int, string>();
        private int selectedIndex = 0;

        private string[] toolbarTexts;

        private Dictionary<int, IEditorPlatformModule>
            elementIdModuleIndex = new Dictionary<int, IEditorPlatformModule>();

        [MenuItem("MikroFramework/Framework/Framework Config")]
        public static void Open() {
            EditorModulizationPlatformEditor editorPlatform = GetWindow<EditorModulizationPlatformEditor>();

            editorPlatform.position = new Rect(Screen.width / 2, Screen.height /2, 1000, 500);

            DefaultModuleCache cache = new DefaultModuleCache();
            AssemblyModuleFactory factory = new AssemblyModuleFactory(typeof(IEditorPlatformModule).Assembly,
                typeof(IEditorPlatformModule));

            editorPlatform.moduleContainer = new ModuleContainer(cache, factory);
            
            editorPlatform.Config();

            editorPlatform.Show();
            
        }

        private void Config() {
            IEnumerable modules = moduleContainer.GetAllModules<IEditorPlatformModule>();


            foreach (IEditorPlatformModule editorPlatformModule in modules) {
                EditorPlatformElement elementInfo = editorPlatformModule.ElementInfo;
                editorPlatformElements[elementInfo.Index] = elementInfo.SidebarName;
                elementIdModuleIndex[elementInfo.Index] = editorPlatformModule;
            }

            int elementLength = editorPlatformElements.Count;
            toolbarTexts = new string[elementLength];

            for (int i = 0; i < elementLength; i++) {
                toolbarTexts[i] = editorPlatformElements[i];
            }
        }

        private void OnGUI() {
            
            int elementLength = editorPlatformElements.Count;

            selectedIndex = GUILayout.Toolbar(selectedIndex, toolbarTexts);

            elementIdModuleIndex[selectedIndex].OnGUI();
        }

        private void ToolBar() {
            
            GUILayout.Window(1, new Rect(2, 2, 220, position.height-4), (windowIndex) => {
                
                GUI.DragWindow();
            },new GUIContent("Modules"));
        }

        
    }
}
