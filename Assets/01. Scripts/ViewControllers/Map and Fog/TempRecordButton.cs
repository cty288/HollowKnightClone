using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;
using UnityEngine.UI;

namespace HollowKnight
{
    public class TempRecordButton : AbstractMikroController<HollowKnight> {
        private Button button;

        private void Awake() {
            button = GetComponent<Button>();
        }

        private void Start() {
            button.onClick.AddListener(this.GetSystem<IMapSystem>().RenderMap);
        }
    }
}
