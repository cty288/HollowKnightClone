using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public class BulletLayoutCircle : AbstractMikroController<HollowKnight> {
        private Player player;
        private Rigidbody2D playerRb;

        [SerializeField] private float lerp = 0.1f;

        private float targetLerp;
        private void Awake() {
            player = Player.Singleton;
            playerRb = player.GetComponent<Rigidbody2D>();
            targetLerp = lerp;
        }

      
        private void FixedUpdate() {
            UpdateLerpSpeed();
            FollowPlayer();
            UpdateRotation();
        }

        private void UpdateRotation() {
            if (playerRb.velocity.x > 0) {
                transform.DOScaleX(1,0.05f);
            }

            if (playerRb.velocity.x < 0)
            {
                transform.DOScaleX(-1, 0.05f);
            }
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
            Vector3 playerPos = player.transform.position;
            this.transform.position = Vector3.Lerp(transform.position, playerPos, targetLerp);
        }
    }
}
