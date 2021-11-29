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

        private void Awake() {
            buffSystem = this.GetSystem<IBuffSystem>();
            text = GetComponent<TMP_Text>();
        }

        private void Update() {
            text.text =
                $"Small Animal Ult Countdown: {buffSystem.GetRemainingTime(BuffType.SmallAnimalUnlimitedBullet)}s";
        }
    }
}
