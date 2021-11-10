using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MikroFramework.CodeGen
{
    public class EditorBinder{

        

        [MenuItem("GameObject/[MikroFramework]Bind this GameObject",
            false, 1)]
        private static void AddBindComponent() {
            if (Selection.gameObjects.Length == 0)
            {
                return;
            }

            GameObject selectedObj = Selection.gameObjects[0];
            
            Component script = selectedObj.GetComponent<Bind>();
            
            if (script != null) {
                return;
            }

            Component bindedComponent = selectedObj.AddComponent(typeof(Bind));
        }

    }
}
