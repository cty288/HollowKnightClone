using System.Collections;
using System.Collections.Generic;
using MikroFramework.ActionKit;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public static class MonoActionExecutor {
        /// <summary>
        /// Execute a MikroAction's Execute() function
        /// Do not use for Sequence
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static void Execute(this MonoBehaviour self, MikroAction action){
            self.StartCoroutine(MikroFramework.ActionKit.CoroutineActionExecutor.Execute(action));
        }

    }
}
