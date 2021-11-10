using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Utilities;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEditor;
using EditorUtility = UnityEditor.EditorUtility;
#endif
using UnityEngine;


namespace MikroFramework.CodeGen
{
    public enum BindInhertFrom {
        Monobehaviour,
        Mikrobehavior,
        AbstractViewController,
        IController,
        CustomClass
    }

    public class Bind : MonoBehaviour {
        

        public string ScriptGenerateRootPath;
        public string ScriptNamespace;
        public string ScriptAssembly;
        public BindInhertFrom InhertFrom;
        public string ArchitectureName;
        public string CustomClassName;

        private void Start() {
            Debug.LogError(
                $"Do not add Bind component to GameObject in Runtime! Remove this component on {gameObject.name}!");
        }
    }

#if UNITY_EDITOR
        [CustomEditor(typeof(Bind))]
        public class BindEditor : Editor {
            

            public override void OnInspectorGUI() {
                Bind script = target as Bind;

                DefaultCodeGenerateRootPath(script);
                
                CodeGenerateAssembly(script);

                CodeGenerateNamespace(script);

                EditorGUILayout.Space(10);
                InheritFrom(script);
                EditorGUILayout.Space(30);

                if (GUILayout.Button("Generate ViewController Code", new GUIStyle("button") {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                })) {
                    Bind(script);
                }
            }





            private static void Bind(Bind bindScript) { 

                GameObject selectedObj = bindScript.gameObject;
                string name = selectedObj.name;

                List<AttributeInfo> attributeInfos = GetAttrbuteInfo(selectedObj);

                if (attributeInfos.Count == 0)
                {
                    return;
                }

                Component script = selectedObj.GetComponent(name);
                string path = bindScript.ScriptGenerateRootPath;

                if (script != null)
                {
                    MonoScript monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour)script);
                    path = AssetDatabase.GetAssetPath(monoScript.GetInstanceID());
                    path = FileUtility.GetParentDirectory(path);
                    AssetDatabase.Refresh();
                }

                string filePath = path + name + "_AutoBind.cs";
                string fileMonoPath = path + name + ".cs";

                if (!File.Exists(fileMonoPath))
                {
                    File.WriteAllText(fileMonoPath, WriteEmptyMono(name,bindScript));
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                EditorPrefs.SetString("BindScript", name);
                EditorPrefs.SetString("BindNamespace",bindScript.ScriptNamespace);
                EditorPrefs.SetString("BindScriptAssembly",bindScript.ScriptAssembly);

                File.WriteAllText(filePath, WriteBindFile(name, attributeInfos,bindScript));
                DestroyImmediate(bindScript);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
              
            }


            [DidReloadScripts]
            private static void BindScriptsToObj()
            {
               
                string scriptName = EditorPrefs.GetString("BindScript", "");
                string scriptNamespace = EditorPrefs.GetString("BindNamespace", "");
                string scriptAssembly = EditorPrefs.GetString("BindScriptAssembly");

            if (!string.IsNullOrEmpty(scriptName)) {
                    EditorPrefs.SetString("BindScript", "");
                    GameObject targetObj = GameObject.Find(scriptName);


                    string nameStr = scriptNamespace + "." + scriptName;

                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                    Type type = null;

                    foreach (Assembly assembly in assemblies)
                    {
                        if (assembly.GetName().Name == scriptAssembly)
                        {
                            type = assembly.GetType(nameStr);
                        }
                    }


                    if (type == null)
                    {
                        Debug.LogError("Could not find type for " + nameStr);
                    }
                    else
                    {

                        Component targetComponent = targetObj.GetComponent(type);

                        if (targetComponent == null)
                        {
                            targetComponent = targetObj.AddComponent(type);
                        }

                        List<AttributeInfo> attributeInfos = BindEditor.GetAttrbuteInfo(targetObj);
                        BindProperties(targetComponent, targetObj, type, attributeInfos);
                    }
                   
                }

             
            }

            private static void BindProperties(object obj, GameObject gameObject, Type type,
                List<AttributeInfo> attributeInfos)
            {

                List<AttributeInfo> tempChild = new List<AttributeInfo>(attributeInfos);
                FieldInfo[] infos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                for (int i = 0; i < infos.Length; i++)
                {
                    string attributeName = infos[i].Name;
                    for (int j = tempChild.Count - 1; j >= 0; --j)
                    {
                        if (tempChild[j].AttributeName == attributeName)
                        {
                            BindProperty(obj, gameObject, infos[i], tempChild[j]);
                            tempChild.Remove(tempChild[j]);
                        }
                    }
                }
            }

            private static void BindProperty(object obj, GameObject gameObject, FieldInfo fieldInfo,
                AttributeInfo attributeInfo) {

                Transform child = gameObject.transform.Find(attributeInfo.PathInParent);
               
                fieldInfo.SetValue(obj, GetObjectByAttributeType(child, attributeInfo.TypeName));
            }


            private static object GetObjectByAttributeType(Transform transform, string typeName)
            {
                if (typeName == "GameObject")
                {
                    return transform.gameObject;
                }
                return transform.gameObject.GetComponent(typeName);
            }



            private static List<AttributeInfo> GetAttrbuteInfo(GameObject obj)
            {
                string name = obj.name;
                Transform[] children = obj.GetComponentsInChildren<Transform>();

                List<AttributeInfo> attributeInfos = new List<AttributeInfo>();

                foreach (Transform child in children)
                {
                    if (child.name.Contains("_"))
                    {
                        string key = child.name.Split('_')[0];
                        string[] value;

                        if (BindingNamingRules.rules.TryGetValue(key.ToLower(), out value))
                        {
                            
                            string fieldName = child.name;
                            fieldName = GetFieldName(fieldName);

                            string typeName = "";

                            foreach (string componentName in value) {
                                if (child.gameObject.GetComponent(componentName) || componentName=="GameObject") {
                                    typeName = componentName;
                                    break;
                                }
                            }

                            string path = "";
                            Transform targetTransform = child;
                            while (targetTransform.parent != obj.transform)
                            {
                                targetTransform = targetTransform.parent;
                                path = targetTransform.name + "/" + path;
                            }

                            path += child.name;

                            AttributeInfo attributeInfo = new AttributeInfo()
                            {
                                AttributeName = fieldName,
                                TypeName = typeName,
                                ObjName = child.name,
                                PathInParent = path.ToString()
                            };
                            attributeInfos.Add(attributeInfo);
                        }
                    }


                }
                return attributeInfos;
            }

            private static string GetFieldName(string fieldName)
            {
                //"Btn_StartGame"
                string[] namePieces = fieldName.Split('_');
                string result = "";
                foreach (string namePiece in namePieces)
                {
                    string firstLetter = namePiece[0].ToString().ToUpper();
                    string factoredName = firstLetter;

                    if (namePiece.Length > 1)
                    {
                        factoredName += namePiece.Substring(1);
                    }

                    result += factoredName;
                }

                return result;
            }

            private static string WriteEmptyMono(string className, Bind bindScript)
            {
                StringBuilder code = new StringBuilder();
                string inheritName = GetInheritName(bindScript);

                WriteDefaultNamespaceCode(code);
                WriteInheritTypeNamespaceCode(code,inheritName,bindScript);

                code.Append("\n");
                code.Append($"namespace {bindScript.ScriptNamespace} {{\n");
              

                code.Append($"\tpublic partial class {className} : {inheritName} {{\n");
                code.Append("\n");
                code.Append("\t}\n");
                code.Append("}");
                return code.ToString();
            }

            

            private static string GetInheritName(Bind bindScript) {
                string inheritName = "";
                switch (bindScript.InhertFrom)
                {
                    case BindInhertFrom.Monobehaviour:
                        inheritName = "MonoBehaviour";
                        break;
                case BindInhertFrom.Mikrobehavior:
                        inheritName = "MikroBehavior";
                        break;
                    case BindInhertFrom.AbstractViewController:
                        inheritName = $"AbstractMikroController<{bindScript.ArchitectureName}>";
                        break;
                    case BindInhertFrom.IController:
                        inheritName = "IController";
                        break;
                    case BindInhertFrom.CustomClass:
                        inheritName = bindScript.CustomClassName;
                        break;
                }

                return inheritName;
            }


            private static string WriteBindFile(string className, List<AttributeInfo> attributeInfos, Bind bindedScript)
            {
                StringBuilder code = new StringBuilder();
                string inheritName = GetInheritName(bindedScript);

                WriteDefaultNamespaceCode(code);
                WriteInheritTypeNamespaceCode(code,inheritName,bindedScript);

                code.Append("\n");
                code.Append($"namespace {bindedScript.ScriptNamespace} {{\n");
                code.Append($"\tpublic partial class {className} : {inheritName} {{\n");
                foreach (AttributeInfo attributeInfo in attributeInfos)
                {
                    code.Append($"\t\t[SerializeField] private {attributeInfo.TypeName} {attributeInfo.AttributeName};\n");
                }

                code.Append("\t}\n");
                code.Append("}");
                return code.ToString();
            }


            private static void WriteDefaultNamespaceCode(StringBuilder code) {
                BindingDefaultNamespaces.DefaultScriptNamespaces.ForEach(namespaceName => {
                    WriteNamespaceCode(code,namespaceName);
                });
            }

            private static void WriteInheritTypeNamespaceCode(StringBuilder code, string inheritName, Bind bindScript) {
                List<string> alreadyExistsNamespace = new List<string>();
                alreadyExistsNamespace.AddRange(BindingDefaultNamespaces.DefaultScriptNamespaces);

                alreadyExistsNamespace.Add(bindScript.ScriptNamespace);
                List<string> inheritTypeNames = ReflectionUtility.GetNamespaceForClass(inheritName);
                

                foreach (string inheritTypeName in inheritTypeNames) {
                    if (!alreadyExistsNamespace.Contains(inheritTypeName)) {
                        WriteNamespaceCode(code,inheritTypeName);
                    }
                }

            }

            private static void WriteNamespaceCode(StringBuilder code, string namespaceName) {
                code.Append($"using {namespaceName};\n");
            }

           
            void DefaultCodeGenerateRootPath(Bind script) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Code Generation Root Path",new GUIStyle() {
                    alignment = TextAnchor.LowerLeft,
                    fontSize = 12,
                });

                script.ScriptGenerateRootPath = EditorGUILayout.TextField(
                    EditorPrefs.GetString("CodeGenRootPath",
                        EditorPrefs.GetString("DefaultCodeGenRootPath", Application.dataPath + "/ViewControllers/")));

                if (GUILayout.Button("Select Path"))
                {

                    string searchPath = EditorUtility.OpenFolderPanel("select path", script.ScriptGenerateRootPath, "") + "/";

                    if (searchPath != "") {
                        EditorPrefs.SetString("CodeGenRootPath", searchPath);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            private string tempGenerateAssemblyInput = "";

            void CodeGenerateAssembly(Bind script) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Script Assembly Name", new GUIStyle()
                {
                    alignment = TextAnchor.LowerLeft,
                    fontSize = 12,
                });


                if (string.IsNullOrEmpty(tempGenerateAssemblyInput)) {
                    script.ScriptAssembly = EditorGUILayout.TextField(
                        EditorPrefs.GetString("CodeGenAssembly",
                            EditorPrefs.GetString("DefaultCodeGenAssembly", "Assembly-CSharp")));
                    tempGenerateAssemblyInput = script.ScriptAssembly;
                }
                else {
                    script.ScriptAssembly = EditorGUILayout.TextField(script.ScriptAssembly);
                }




                if (GUILayout.Button("Save")) {
                    
                    EditorPrefs.SetString("CodeGenAssembly", script.ScriptAssembly);
                }

                EditorGUILayout.EndHorizontal();
            }



            private string tempGenerateNamespaceInput = "";

            void CodeGenerateNamespace(Bind script)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Script Namespace Name", new GUIStyle()
                {
                    alignment = TextAnchor.LowerLeft,
                    fontSize = 12,
                });


                if (string.IsNullOrEmpty(tempGenerateNamespaceInput))
                {
                    script.ScriptNamespace = EditorGUILayout.TextField(
                        EditorPrefs.GetString("CodeGenNamespace",
                            EditorPrefs.GetString("DefaultCodeGenNamespace", "MikroFramework.GeneratedCode")));
                    tempGenerateNamespaceInput = script.ScriptNamespace;
                }
                else
                {
                    script.ScriptNamespace = EditorGUILayout.TextField(script.ScriptNamespace);
                }




                if (GUILayout.Button("Save"))
                {

                    EditorPrefs.SetString("CodeGenNamespace", script.ScriptNamespace);
                }

                EditorGUILayout.EndHorizontal();
            }

            private int tempEnumPopupInput = -1;
            void InheritFrom(Bind script) {
                EditorGUILayout.LabelField("Script Inherit From", new GUIStyle()
                {
                    alignment = TextAnchor.LowerLeft,
                    fontSize = 12,
                });

                EditorGUILayout.BeginHorizontal();

                if (tempEnumPopupInput==-1)
                {
                    int savedPopupSelection = EditorPrefs.GetInt("CodeGenInheritFrom",
                        EditorPrefs.GetInt("DefaultCodeGenInheritFrom", 0));

                    script.InhertFrom =(BindInhertFrom)EditorGUILayout.EnumPopup((BindInhertFrom)savedPopupSelection);

                    tempEnumPopupInput = (int) script.InhertFrom;
                }
                else
                {
                    script.InhertFrom =(BindInhertFrom) EditorGUILayout.EnumPopup(script.InhertFrom);
                }

                EditorPrefs.SetInt("CodeGenInheritFrom", (int) script.InhertFrom);


                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (script.InhertFrom == BindInhertFrom.AbstractViewController)
                {
                    CodeGenAbstractViewControllerInputBox(script);
                }
                else if (script.InhertFrom == BindInhertFrom.CustomClass)
                {
                    CodeGenCustomClass(script);
                }

                EditorGUILayout.EndHorizontal();

            }

            private string tempVCInputBox = "";
            private void CodeGenAbstractViewControllerInputBox(Bind script) {
                EditorGUILayout.LabelField("Architecture Class Name", new GUIStyle()
                {
                    alignment = TextAnchor.LowerLeft,
                    fontSize = 12,
                });


                if (string.IsNullOrEmpty(tempVCInputBox))
                {
                    script.ArchitectureName = EditorGUILayout.TextField(
                        EditorPrefs.GetString("CodeGenArchitecture",""));

                    tempVCInputBox = script.ArchitectureName;
                }else
                {
                    script.ArchitectureName = EditorGUILayout.TextField(script.ArchitectureName);
                }
                
                if (GUILayout.Button("Save")) {

                    EditorPrefs.SetString("CodeGenArchitecture", script.ArchitectureName);
                }
            }




            private string tempClassName = "";
            private void CodeGenCustomClass(Bind script)
            {
                EditorGUILayout.LabelField("Class Name", new GUIStyle()
                {
                    alignment = TextAnchor.LowerLeft,
                    fontSize = 12,
                });


                if (string.IsNullOrEmpty(tempClassName))
                {
                    script.CustomClassName = EditorGUILayout.TextField(
                        EditorPrefs.GetString("CodeGenCustomClass", ""));

                    tempClassName = script.CustomClassName;
                }
                else
                {
                    script.CustomClassName = EditorGUILayout.TextField(script.CustomClassName);
                }

                if (GUILayout.Button("Save")) {
                    EditorPrefs.SetString("CodeGenCustomClass", script.CustomClassName);
                }
            }
        }

#endif
}

