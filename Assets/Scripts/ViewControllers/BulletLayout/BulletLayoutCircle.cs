using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public class BulletLayoutCircle : AbstractMikroController<HollowKnight> {
        private Player player;

        [SerializeField] private float lerp = 0.1f;

        private float targetLerp;
        private void Awake() {
            player = Player.Singleton;
            targetLerp = lerp;
        }

      
        private void FixedUpdate() {
            UpdateLerpSpeed();
            FollowPlayer();
        }

        private void UpdateLerpSpeed() {
            float speed = player.Speed.Value;
            if (speed <= 1 && speed >= 0)
            {
                targetLerp = lerp;
            }
            else
            {
                targetLerp = Mathf.Lerp(targetLerp, 1, 0.01f);

            }
        }

        private void FollowPlayer() {
            Debug.Log(targetLerp);
            Vector3 playerPos = player.transform.position;
            this.transform.position = Vector3.Lerp(transform.position, playerPos, targetLerp);
        }
    }
}
