using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public class ChangeSceneBG : AbstractMikroController<HollowKnight>
    {
        private void Start() {
            this.RegisterEvent<OnSceneSwitch>(OnSceneSwitch).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnSceneSwitch(OnSceneSwitch obj) {
            GetComponent<Animation>().Play();
        }
    }
}
