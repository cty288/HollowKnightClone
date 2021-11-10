using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MikroFramework.Architecture;
using UniRx;
using UnityEngine;

namespace MikroFramework
{
    public class ReflectionUtility {
        /// <summary>
        /// Return the Assembly object of assembly "Assembly-CSharp-Editor"
        /// </summary>
        public static Assembly EditorAssembly {
            get {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly editorAssembly =
                    assemblies.First(assembly => assembly.GetName().Name.Equals("Assembly-CSharp-Editor"));
                return editorAssembly;
            }
        }

        

        public static List<string> GetNamespaceForClass(string className) {
            List<string> classNames;
            
            //consider generic type
            if (className.Contains('<')) {
                classNames = className.Split(new char[2] {'<', '>'}).ToList();
                for (int i = 0; i < classNames.Count; i++) {
                    string element = classNames[i];
                    if (string.IsNullOrEmpty(element) || element == "<" || element == ">") {
                        classNames.RemoveAt(i);
                    }
                }
            }
            else {
                classNames = new[] {className}.ToList();
            }

            List<string> results = new List<string>();
            
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            foreach (string name in classNames) {
                if (name == "AbstractMikroController" || name== "IController") {
                    results.Add("MikroFramework.Architecture");
                }else if (name=="MikroBehavior") {
                    results.Add("MikroFramework");
                }else {
                    foreach (Assembly assembly in assemblies) {
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types) {
                            if (type.Name == name) {
                                results.Add(type.Namespace);
                                break;
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
