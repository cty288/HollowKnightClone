using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Singletons;
using UnityEngine;

namespace MikroFramework.ActionKit
{
    public class ActionPlayer:MikroSingleton<ActionPlayer> {
       private ActionPlayer() { }

        public void RegisterUpdate(Action onUpdate) {
            UpdateActionExecutor.ActionKitUpdateTrigger.Singleton.OnUpdate += onUpdate;
        }

        public void UnRegisterUpdate(Action onUpdate) {
            UpdateActionExecutor.ActionKitUpdateTrigger.Singleton.OnUpdate -= onUpdate;
        }


        public void Play(MikroAction action)
        {
            action.Execute();
        }

    }
}
