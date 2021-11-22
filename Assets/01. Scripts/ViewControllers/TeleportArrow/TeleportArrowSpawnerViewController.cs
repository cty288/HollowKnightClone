using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class TeleportArrowSpawnerViewController : AbstractMikroController<HollowKnight> {
        [SerializeField] 
        private GameObject arrowPrefab;

        private void Awake() {
            this.RegisterEvent<OnTeleportStart>(OnTeleportStart).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnTeleportStart(OnTeleportStart e) {
            
        }
    }
}
