using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class BossStone : AbstractMikroController<HollowKnight> {
        public int damage = 3;

        private Rigidbody2D rigidbody;
        private Animator animator;

        private bool hit = false;
        private void Awake() {
            Destroy(this.gameObject,10f);
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other) {

            if (!other.isTrigger && !hit) {

                if (other.gameObject.TryGetComponent<IEnemyViewControllerAbsorbable>(
                    out IEnemyViewControllerAbsorbable enemy)) {
                    return;
                }

                hit = true;
                animator.SetTrigger("Hit");
                rigidbody.gravityScale = 0;
                rigidbody.velocity = Vector2.zero;
                if (other.gameObject == Player.Singleton.gameObject) {
                    this.GetModel<IPlayerModel>().ChangeHealth(-damage);
                }
            }
        }

        public void OnHitAnimationEnd() {
            Destroy(this.gameObject);
        }
    }
}
