using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using TMPro;
using UnityEngine;

namespace HollowKnight
{
    public class TempUltChargeUI : AbstractMikroController<HollowKnight> {

        private TMP_Text text;
        private void Awake() {
            text = GetComponent<TMP_Text>();
            OnUltChange(0,0);
        }

        private void Start() {
            this.GetModel<IPlayerModel>().UltChargeAccumlated.RegisterOnValueChaned(OnUltChange)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnUltChange(int old, int newUlt) {
            text.text = $"Ult Charged: {newUlt} / {this.GetModel<IPlayerConfigurationModel>().MaxUltChargeNeeded}";
        }
    }
}
