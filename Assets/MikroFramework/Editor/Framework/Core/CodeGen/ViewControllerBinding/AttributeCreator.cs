/*
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Utilities;
using NHibernate.Event.Default;
using NHibernate.Hql.Ast.ANTLR.Util;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace MikroFramework.CodeGen
{
    public class AttributeCreator {

        private static string NAMESPACE = "MikroFramework.GeneratedCode";
        private static string ASSEMBLY = "MikroFramework";

        [MenuItem("GameObject/[MikroFramework]Auto Create View Controller Code",
            false,0)]
        public static void AutoAttributeCreator() {
            if (Selection.gameObjects.Length == 0) {
                return;
            }

            GameObject selectedObj = Selection.gameObjects[0];
            string name = selectedObj.name;

            List<AttributeInfo> attributeInfos = GetAttrbuteInfo(selectedObj);

            if (attributeInfos.Count == 0) {
                return;
            }

            Component script = selectedObj.GetComponent(name);
            string path = Application.dataPath + "/MikroFramework/Runtime/";

            if (script != null) {
                MonoScript monoScript = MonoScript.FromMonoBehaviour((MonoBehaviour) script);
                path = AssetDatabase.GetAssetPath(monoScript.GetInstanceID());
                path = FileUtility.GetParentDirectory(path);
                AssetDatabase.Refresh();
            }

            string filePath = path + name + "_AutoBind.cs";
            string fileMonoPath = path + name + ".cs";

            if (!File.Exists(fileMonoPath)) {
                File.WriteAllText(fileMonoPath,WriteEmptyMono(name));
            }

            if (File.Exists(filePath)) {
                File.Delete(filePath);
            }

            File.WriteAllText(filePath,WriteBindFile(name,attributeInfos));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetString("BindScript",name);
        }

        [DidReloadScripts]
        private static void BindScriptsToObj() {
            string scriptName = EditorPrefs.GetString("BindScript", "");

            if (!string.IsNullOrEmpty(scriptName)) {
                EditorPrefs.SetString("BindScript", "");
                GameObject targetObj=GameObject.Find(scriptName);

                
                string nameStr =NAMESPACE+"."+ scriptName;

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                Type type = null;

                foreach (Assembly assembly in assemblies) {
                    if (assembly.GetName().Name == ASSEMBLY) {
                         
                        type = assembly.GetType(nameStr);
                        Debug.Log(type.Name);
                         
                    }
                }


                if (type == null) {
                    Debug.LogError("Could not find type for "+ nameStr);
                }
                else {

                    Component targetComponent = targetObj.GetComponent(type);

                    if (targetComponent == null) {
                        targetComponent = targetObj.AddComponent(type);
                    }

                    List<AttributeInfo> attributeInfos = GetAttrbuteInfo(targetObj);
                    BindProperties(targetComponent, targetObj, type, attributeInfos);
                }

            }
        }

        private static void BindProperties(object obj, GameObject gameObject, Type type,
            List<AttributeInfo> attributeInfos) {
            
            List<AttributeInfo> tempChild = new List<AttributeInfo>(attributeInfos);
            FieldInfo[] infos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < infos.Length; i++) {
                string attributeName = infos[i].Name;
                for (int j = tempChild.Count-1; j >= 0; --j) {
                    if (tempChild[j].AttributeName == attributeName) {
                        BindProperty(obj,gameObject,infos[i],tempChild[j]);
                        tempChild.Remove(tempChild[j]);
                    }
                }
            }
        }

        private static void BindProperty(object obj, GameObject gameObject, FieldInfo fieldInfo,
            AttributeInfo attributeInfo) {

            Transform child = gameObject.transform.Find(attributeInfo.PathInParent);
            Debug.Log(child.name+"    "+
                      attributeInfo.TypeName+"     "+
                      attributeInfo.PathInParent);
            fieldInfo.SetValue(obj,GetObjectByAttributeType(child,attributeInfo.TypeName));
        }

        private static object GetObjectByAttributeType(Transform transform, string typeName) {
            if (typeName == "GameObject") {
                return transform.gameObject;
            }
            return transform.gameObject.GetComponent(typeName);
        }



        private static List<AttributeInfo> GetAttrbuteInfo(GameObject obj) {
            string name = obj.name;
            Transform[] children = obj.GetComponentsInChildren<Transform>();

            List<AttributeInfo> attributeInfos = new List<AttributeInfo>();

            foreach (Transform child in children) {
                if (child.name.Contains("_")) {
                    string key = child.name.Split('_')[0];
                    string value;

                    if (BindingNamingRules.rules.TryGetValue(key, out value)) {
                        string fieldName = child.name;
                        fieldName = GetFieldName(fieldName);

                        string path = "";
                        Transform targetTransform = child;
                        while (targetTransform.parent != obj.transform) {
                            targetTransform = targetTransform.parent;
                            path = targetTransform.name + "/" + path;
                        }

                        path += child.name;

                        AttributeInfo attributeInfo = new AttributeInfo() {
                            AttributeName = fieldName,
                            TypeName = value,
                            ObjName = child.name,
                            PathInParent = path.ToString()
                        };
                        attributeInfos.Add(attributeInfo);
                    }
                }

               
            }
            return attributeInfos;
        }

        private static string GetFieldName(string fieldName) {
            //"Btn_StartGame"
            string[] namePieces = fieldName.Split('_');
            string result = "";
            foreach (string namePiece in namePieces) {
                string firstLetter= namePiece[0].ToString().ToUpper();
                string factoredName = firstLetter;

                if (namePiece.Length > 1) {
                    factoredName += namePiece.Substring(1);
                }
                
                result += factoredName;
            }

            return result;
        }

        private static string WriteEmptyMono(string className) {
            StringBuilder code = new StringBuilder();

            code.Append("using System;\n");
            code.Append("using System.Collections;\n");
            code.Append("using UnityEngine;\n");
            code.Append("using UnityEngine.UI;\n");
            code.Append("\n");
            code.Append($"namespace {NAMESPACE} {{\n");
            code.Append($"\tpublic partial class {className} : MonoBehaviour{{\n");
            code.Append("\n");
            code.Append("\t}\n");
            code.Append("}");
            return code.ToString();
        }


        private static string WriteBindFile(string className, List<AttributeInfo> attributeInfos) {
            StringBuilder code = new StringBuilder();
            code.Append("using System;\n");
            code.Append("using System.Collections;\n");
            code.Append("using UnityEngine;\n");
            code.Append("using UnityEngine.UI;\n");
            code.Append("\n");
            code.Append($"namespace {NAMESPACE} {{\n");
            code.Append($"\tpublic partial class {className} : MonoBehaviour{{\n");
            foreach (AttributeInfo attributeInfo in attributeInfos) {
                code.Append($"\t\t[SerializeField] private {attributeInfo.TypeName} {attributeInfo.AttributeName};\n");
            }

            code.Append("\t}\n");
            code.Append("}");
            return code.ToString();
        }
    }

}
*/