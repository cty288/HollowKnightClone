using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class ShockWave : AbstractMikroController<HollowKnight> {
        [SerializeField] private float lastTime = 20f;

        public float speed = 10;

        [SerializeField] private float damage = 8;
        public bool FaceRight = false;

        private bool triggered = false;
        private Animator animator;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        private void Start() {
            Destroy(this.gameObject,lastTime);
        }

        private void FixedUpdate() {
            float direction = FaceRight ? 1 : -1;
            transform.position += new Vector3(speed * direction * Time.deltaTime, 0, 0);
        }

        public void OnAnimationFinished() {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject == Player.Singleton.gameObject) {
                if (!triggered) {
                    triggered = true;
                    this.GetModel<IPlayerModel>().ChangeHealth(-damage);
                    animator.SetTrigger("Hit");
                }
                
            }
        }
    }
}
