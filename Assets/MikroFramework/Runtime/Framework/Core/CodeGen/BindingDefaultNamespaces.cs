using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework;

namespace MikroFramework.CodeGen {
    public static class BindingDefaultNamespaces {
        
        public static List<string> DefaultScriptNamespaces = new List<string>() {
            "System",
            "System.Collections",
            "UnityEngine",
            "UnityEngine.UI",
            "MikroFramework",
            "TMPro", //comment this if you don't have TMP for this project
        };
    }
}
