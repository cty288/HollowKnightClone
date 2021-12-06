using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using TMPro;
using UnityEngine;

namespace HollowKnight
{
    public class TempUltTimeUI : AbstractMikroController<HollowKnight> {
        private IBuffSystem buffSystem;
        private TMP_Text text;
        public BuffType BuffType;
        public string Prefix = "Small Animal Ult Countdown: ";

        private void Awake() {
            buffSystem = this.GetSystem<IBuffSystem>();
            text = GetComponent<TMP_Text>();
        }

        private void Update() {
            text.text =
                $"{Prefix}{buffSystem.GetRemainingTime(BuffType)}s";
        }
    }
}
