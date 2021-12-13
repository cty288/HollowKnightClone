using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.UIElements;

namespace HollowKnight
{
    public class BossRoomWallViewController : AbstractMikroController<HollowKnight> {
        private BoxCollider2D collider;

        private void Awake() {
            this.RegisterEvent<OnPlayerEnterBossRoom>(OnPlayerEnterBossRoom)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnPlayerRespawned>(OnPlayerRespawned).UnRegisterWhenGameObjectDestroyed(gameObject);

            collider = GetComponent<BoxCollider2D>();
            collider.enabled = false;
        }

        private void OnPlayerRespawned(OnPlayerRespawned e) {
            collider.enabled = false;
        }

        private void OnPlayerEnterBossRoom(OnPlayerEnterBossRoom e) {
            collider.enabled = true;
        }
    }
}
