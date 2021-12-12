using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class RespawnPointRecordViewController : AbstractMikroController<HollowKnight> {
        [SerializeField] private int respawnNumber = 0;

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject == Player.Singleton.gameObject) {
                GameManager.Singleton.RespawnPointRecord(respawnNumber);
            }
        }
    }
}
