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
        private bool played = false;

        private void Awake() {
            Destroy(this.gameObject,10f);
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (!other.collider.isTrigger && !played) {
                animator.SetTrigger("Hit");
                Debug.Log("Stone hit");
                played = true;
               // rigidbody.gravityScale = 0;
              //  rigidbody.velocity = Vector2.zero;
                
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
