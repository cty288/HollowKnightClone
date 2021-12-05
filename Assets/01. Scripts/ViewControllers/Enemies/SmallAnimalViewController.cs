using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HollowKnight
{

    public class MonsterViewController  : AbstractAbsorbableEnemy<ChargeMonsterConfigurtion>, IEnemyViewControllerAttackable{
        protected override void Start() {
           // absorbableConfiguration
           //weaponInfo
        }

        private void Update() {
            if (FSM.CurrentState.name == "idle") {
                //idle


               // if (widea) {
                    configurationItem.FSM.HandleEvent("233");
                //}
            }
        }

        
        
        protected override void OnFSMStateChanged(string prevEvent, string newEvent) {
            
        }
    }

    public class SmallAnimalViewController<T> : AbstractAbsorbableEnemy<T>, IEnemyViewControllerAttackable where T:EnemyConfigurationItem,new(){
        [SerializeField] protected float speed = 10f;

        [SerializeField] protected Vector2 RangeX;

        [SerializeField]
        protected bool moveLeft = true;
        protected bool directionChanged = false;


        [SerializeField] 
        protected Transform normalAttackBulletSpawnPosition;

        [SerializeField] protected GameObject bulletPrefab;

        private int bulletShot = 0;
        protected override void Awake() {
            base.Awake();
            shakeParent = spriteRenderer.transform.parent;
        }

        protected override void Start() {
            base.Start();
            this.RegisterEvent<SmallAnimalNormalAttackCommand.OnSmallAnimalBulletConsumed>(OnBulletConsumed)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnBulletConsumed(SmallAnimalNormalAttackCommand.OnSmallAnimalBulletConsumed e) {
            if (e.WeaponInfo == weaponInfo && IsDie) {
                //spawn bullet
                float spawnX = normalAttackBulletSpawnPosition.position.x + Random.Range(-0.1f, 0.1f);
                float spawnY = normalAttackBulletSpawnPosition.position.y;

                this.SendCommand<SpawnBulletCommand>(SpawnBulletCommand.Allocate(e.TargetGameObject, 
                    new Vector2(spawnX, spawnY),
                    bulletPrefab, e.ShootInstant,e.WeaponInfo.AttackDamage.Value));
              
            }
            
        }

        protected void Update() {
            base.Update();
            if (transform.position.x >= RangeX.x && transform.position.x <= RangeX.y) {
                directionChanged = false;
            }
           
        }

        private void FixedUpdate() {
            if (absorbableConfiguration.Health.Value > 0) {
                float targetSpeed = moveLeft ? -speed : speed;
                rigidbody.velocity = new Vector2(targetSpeed, 0);

                if (!directionChanged)
                {
                    if (transform.position.x < RangeX.x)
                    {
                        moveLeft = false;
                        directionChanged = true;
                        transform.DOScaleX(-1, 0);
                    }

                    if (transform.position.x > RangeX.y)
                    {
                        moveLeft = true;
                        directionChanged = true;
                        transform.DOScaleX(1, 0);
                    }
                }
            }

        }

        public override void OnDie() {
            base.OnDie();
            transform.DOScaleY(-1, 0.1f);
            
        }

        public override void OnStartPrepareAbsorb() {
            StartCoroutine(Float());
        }

        private Tween moveTween;
        public override void OnAbsorbInterrupt() {
            StopAllCoroutines();
            Debug.Log("Mouse Interrupt");
            if (moveTween != null) {
                moveTween.Kill();
            }
            spriteRenderer.gameObject.transform.DOLocalMoveY(0, 0.3f);
        }

        private IEnumerator Float() {
            yield return new WaitForSeconds(0.83f);

            spriteRenderer.transform.parent.DOShakePosition(1.67f, 0.2f, 20, 100);

            moveTween = spriteRenderer.gameObject.transform.DOMoveY(spriteRenderer.gameObject.transform.position.y+3, 1.67f); 
            
        }

        public override void OnAbsorbed() {
            base.OnAbsorbed();
            if (moveTween != null)
            {
                moveTween.Kill();
            }
            transform.DOScaleY(1, 0.1f);
        }


        public override void OnDropped() {
            base.OnDropped();
            transform.DOScaleY(-1, 0.1f);
        }

        public override void OnBulletShot(int number) {
            base.OnBulletShot(number);
        }

        protected override void OnFSMStateChanged(string prevEvent, string newEvent) {
            
        }
    }
}
