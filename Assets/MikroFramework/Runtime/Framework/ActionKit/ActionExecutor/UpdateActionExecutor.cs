using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public static class UpdateActionExecutor {
        public static void Execute(MikroAction action) { 
            action.Execute();
        }

        [MonoSingletonPath("[FrameworkPersistent]/[ActionKit]/ActionKitUpdater")]
        public class ActionKitUpdateTrigger : MonoPersistentMikroSingleton<ActionKitUpdateTrigger>
        {
            public Action OnUpdate = () => { };

            private void Update()
            {
                OnUpdate.Invoke();
            }
        }
    }
}
