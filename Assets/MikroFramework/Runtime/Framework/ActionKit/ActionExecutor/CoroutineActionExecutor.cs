using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public static class CoroutineActionExecutor
    {
        /// <summary>
        /// Execute a MikroAction's Execute() function
        /// Do not use for Sequence
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerator Execute(MikroAction action)
        {
            action.Execute();
            yield return null;
        }
    }
}
