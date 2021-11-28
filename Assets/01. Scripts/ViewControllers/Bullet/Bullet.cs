using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public enum BulletState {
        Preparing,
        Shooting,
        Hit
    }
    public class Bullet : AbstractMikroController<HollowKnight> {
        public Transform Target;
        public bool ShootInstant = true;

        private Animator animator;

        private BulletState bulletState = BulletState.Preparing;

        public int Damage;

        [SerializeField] private float shootSpeed = 30f;
        private void Awake() {
            animator = GetComponent<Animator>();
            this.RegisterEvent<OnSmallAnimalChargeReleased>(OnChargeReleased).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Start() {
            
        }

        private void OnChargeReleased(OnSmallAnimalChargeReleased obj)
        {
            if (bulletState == BulletState.Preparing) {
                ShootInstant = true;
                Shoot();
            }
        }
        private void Update() {
            
            
            if (bulletState == BulletState.Preparing) {
                Aim();
            }
            if (bulletState == BulletState.Shooting) {
                Aim();
                //transform.DODynamicLookAt(Target.position, 0,,);
                transform.position += transform.right * shootSpeed * Time.deltaTime;
            }
           
        }

        void Aim() {
            Vector2 resultVec = Target.position - new Vector3(transform.position.x, transform.position.y, 0);

            float angle = Mathf.Atan2(resultVec.y, resultVec.x) * 180 / Mathf.PI;


            transform.DORotate(new Vector3(0, 0, angle), 0);
        }

        public void OnPrepareFinished() {
            if (ShootInstant) {
                Shoot();
            }
        }

        private void Shoot() {
            bulletState = BulletState.Shooting;
            animator.SetTrigger("Shoot");
            
            //float time = Mathf.Abs(Vector2.Distance(transform.position, Target.position)) / shootSpeed;
            
           // transform.DOMove(Target.position, time).SetEase(Ease.Linear);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            if (bulletState == BulletState.Shooting) {
                if (other.collider.gameObject) {
                    bulletState = BulletState.Hit;
                    
                    animator.SetTrigger("Hit");

                    if (other.collider.gameObject.TryGetComponent<IEnemyViewControllerAttackable>(out IEnemyViewControllerAttackable attackable)) {

                        
                        Debug.Log($"Bullet shoot an attackable {attackable.Attackable.Health.Value}," +
                                   $"with damage: {Damage}");
                        this.SendCommand<HurtEnemyCommand>(HurtEnemyCommand.Allocate(attackable.Attackable,Damage));
                    }
                }
            }
            
        }

        public void OnHitFinished() {
            Destroy(this.gameObject);
        }
    }
}
