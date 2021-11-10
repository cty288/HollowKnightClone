using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.Singletons
{
    public class MonoSingletonPath : Attribute {
        private string pathInHierarchy;

        public MonoSingletonPath(string pathInHierarchy) {
            this.pathInHierarchy = pathInHierarchy;
        }

        public string PathInHierarchy => pathInHierarchy;



    }
}
