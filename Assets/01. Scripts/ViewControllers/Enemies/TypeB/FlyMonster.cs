using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HollowKnight
{
    public class FlyMonster : AbstractAbsorbableCanAttackEnemy<FlyMonsterConfiguration, FlyMonsterConfiguration.FlyMonsterStages>
    {
        
        [SerializeField] private float speed;
        [SerializeField] private Vector2 startLocation;
        private Vector2 nextSpot;
        [SerializeField] private GameObject target;
        [SerializeField] private float startWaitTime;
       

        [SerializeField] private Animator animator;
        [SerializeField] private float patrollRangeX;
        [SerializeField] private float patrollRangeY;
        [SerializeField] private Vector3 spriteScale;

        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject enemyBulletPrefab;
        [SerializeField] private bool canFire = true;
        [SerializeField] private bool facingRight = true;

        [SerializeField] private SpriteRenderer aliveOutlineSpriteRenderer;
        [SerializeField] private SpriteRenderer deathOutlineSpriteRenderer;

        private SpriteRenderer aliveSpriteRenderer;
        [SerializeField]
        private SpriteRenderer deathWeaponSpriteRenderer;

      
        private Trigger2DCheck attackTrigger2DCheck;

        private float min_x;
        private float max_x;
        private float min_y;
        private float max_y;
        private float waitTime;
        private float attackTimer;
        private float p;
        float startLocation_y;
        Vector2 nextAttackSpotCenter;

        private void Awake()
        {
            base.Awake();
            startLocation = transform.position;
            min_x = startLocation.x - patrollRangeX;
            max_x = startLocation.x + patrollRangeX;
            min_y = startLocation.y - patrollRangeY;
            max_y = startLocation.y + patrollRangeY;
            deathReady = false;
            waitTime = startWaitTime;
            spriteScale = transform.localScale;
            nextAttackSpotCenter = transform.position;
            startLocation_y = transform.position.y;
           
            attackTrigger2DCheck = GetComponentInChildren<Trigger2DCheck>();
            aliveSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        

        protected override void OnMouseOver() { }

        protected override void OnMouseExit() { }

        private void OnBulletConsumed(OnHumanoidBulletConsumed e)
        {
            if (e.WeaponInfo == weaponInfo && IsDie)
            {
                //spawn bullet
                float spawnX = bulletSpawnPosition.position.x + Random.Range(-0.1f, 0.1f);
                float spawnY = bulletSpawnPosition.position.y;
                Debug.Log("Bullet consumed");
                this.SendCommand<SpawnThornCommand>(SpawnThornCommand.Allocate(e.TargetPos,
                    new Vector2(spawnX, spawnY),
                    bulletPrefab, e.ShootInstant, e.WeaponInfo.AttackDamage.Value));
            }
        }

        protected override void Start() {
            base.Start();
            nextSpot = DecideDistance(transform.position);
            this.RegisterEvent<OnHumanoidBulletConsumed>(OnBulletConsumed).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Patrolling() {
            if (!IsDie) {
                rigidbody.gravityScale = 0;
            }
            
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(nextSpot.x, nextSpot.y), speed * Time.deltaTime);

            if ((nextSpot.x - transform.position.x) < 0) {
                spriteScale.x = 1;
            }
            else if ((nextSpot.x - transform.position.x) > 0) {
                spriteScale.x = -1;
            }

            if (Vector2.Distance(transform.position, nextSpot) < 0.25f)
            {   
                if (waitTime <= 0)
                {
                    nextSpot = DecideDistance(transform.position);
                    waitTime = startWaitTime;
                }
                else
                {
                    //animator.SetInteger("EnemyState", 1);
                    waitTime -= Time.deltaTime;
                }
            }
        }

        private void Fire()
        {
            if (attackTimer <= 0)
            {
                animator.SetInteger("EnemyState", 2);
                GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
                attackTimer = GetCurrentAttackRate();
                canFire = false;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }
        }

        private bool CheckReached(Vector2 nextSpot)
        {
            if (Vector2.Distance(transform.position, nextSpot) <= 0.5)
                return true;
            return false;
        }
        private bool CheckPlayerDistance()
        {
            if (Vector2.Distance(transform.position, target.transform.position) <= 5)
                return true;
            return false;
        }

        private IEnumerator Dodging()
        {
            float moveRange = 2f;

            if ((nextAttackSpotCenter.x - transform.position.x) <= 0) spriteScale.x = 1;
            else if ((nextAttackSpotCenter.x - transform.position.x) > 0) spriteScale.x = -1;

            if (CheckPlayerDistance())
            {
                transform.position += new Vector3 (nextAttackSpotCenter.x, nextAttackSpotCenter.y) * 1.5f * Time.deltaTime;
            }

            else if (!CheckReached(nextAttackSpotCenter))
            {
                transform.position = Vector2.MoveTowards(transform.position,
                  nextAttackSpotCenter, speed * Time.deltaTime);
            }

            //if (CheckReached(nextAttackSpotCenter))
            
            p = Random.Range(0f, 100f);

            yield return new WaitForSeconds(.7f);

            nextAttackSpotCenter = new Vector2(Random.Range(-2f, 2f), Random.Range(startLocation_y - 1, startLocation_y + 1));
            canFire = true;
        }


        private IEnumerator CoroutineAttack()
        {
            animator.SetInteger("EnemyState", 2);
            Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(0.5f);
            Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
            animator.SetInteger("EnemyState", 1);
        }

        private Vector2 DecideDistance(Vector2 currentSpot)
        {
            Vector2 randomSpot = new Vector2(Random.Range(min_x, max_x), Random.Range(min_y, max_y));
            while (Vector2.Distance(currentSpot, randomSpot) < 5f)
            {
                randomSpot = new Vector2(Random.Range(min_x, max_x), Random.Range(min_y, max_y));
            }

            return randomSpot;
        }

      

        private void Update()
        {
            base.Update();
            if (aliveOutlineSpriteRenderer.enabled)
            {
                outlineSpriteRenderer.sprite = spriteRenderer.sprite;
            }

            CheckMouseHover();
            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
            {
                spriteRenderer.color = Color.white;
            }
            if (attackTrigger2DCheck.Triggered) {
                target = Player.Singleton.gameObject;
                TriggerEvent(FlyMonsterConfiguration.FlyMonsterEvents.PlayerInRange);
            }
            CheckPlayer();
            transform.localScale = spriteScale;
        }

        private void CheckMouseHover() {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider) {
                if (hit.collider is CircleCollider2D && hit.collider.gameObject == this.gameObject) {
                    OnMouseHover();
                    return;
                }
            }

            OnMouseLeave();
        }

        private void OnMouseLeave() {
            if (this.GetSystem<IAbsorbSystem>().AbsorbState == AbsorbState.NotAbsorbing)
            {
                outlineSpriteRenderer.enabled = false;
            }
        }

        private void OnMouseHover() {
            IAbsorbSystem absorbSystem = this.GetSystem<IAbsorbSystem>();
            IAttackSystem attackSystem = this.GetSystem<IAttackSystem>();

            if (absorbSystem.AbsorbState != AbsorbState.NotAbsorbing)
            {
                if (absorbSystem.AbsorbingGameObject != this.gameObject)
                {
                    return;
                }
            }

            if (attackSystem.AttackState != AttackState.NotAttacking)
            {
                return;
            }

            if (absorbableConfiguration.Absorbed.Value)
            {
                return;
            }
            outlineSpriteRenderer.enabled = true;
        }

        public override void OnAbsorbed() {
            base.OnAbsorbed();
            int spriteIndex = weaponInfo.MaxBulletCount - weaponInfo.BulletCount.Value;
            spriteRenderer.sprite = bulletStateSprites[spriteIndex];
        }

        protected override void OnSeePlayer() { }

        public override void OnAttackingStage(Enum attackStage) { }

        public override void OnDie() {
            base.OnDie();
            TriggerEvent(FlyMonsterConfiguration.FlyMonsterEvents.Killed);
            animator.SetTrigger("Die");
            Debug.Log("Die");
        }

        public void OnStartDieAnimationFinished() {
            this.rigidbody.gravityScale = 1;
            Debug.Log(this.rigidbody.gravityScale);
        }

        public override void OnFSMStage(FlyMonsterConfiguration.FlyMonsterStages currentStage) {
            if (currentStage == FlyMonsterConfiguration.FlyMonsterStages.Patrolling)
            {
                animator.SetInteger("EnemyState", 1);
                Patrolling();
            }
            else if (currentStage == FlyMonsterConfiguration.FlyMonsterStages.Engaging)
            {
                if (canFire) {
                    if (this.GetModel<IPlayerModel>().Health.Value > 0) {
                        Fire();
                    }
                    
                }
                else {
                    StartCoroutine(Dodging());
                }
            }
           
        }

        protected override void OnNotSeePlayer() {
            
        }

        void Flip()
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        void CheckPlayer()
        {
            if (target != null)
            {
                if (Vector2.Distance(target.transform.position, this.transform.position) >= patrollRangeX)
                {
                    TriggerEvent(FlyMonsterConfiguration.FlyMonsterEvents.PlayerOutRange);
                }
            }
        }

        [SerializeField] private LayerMask fallDetectionLayers;
        private void OnCollisionEnter2D(Collision2D other) {
            if (IsDie) {
                if (!deathReady) {
                    if (PhysicsUtility.IsInLayerMask(other.gameObject, fallDetectionLayers)) {
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
                        {
                            animator.SetTrigger("Die_HitGround");
                        }
                    }
                    
                }
            }
        }
        private float hurtTimer = 0.3f;
        public override void OnAttacked(float damage) {
            base.OnAttacked(damage);
            if (Attackable.Health.Value > 0)
            {
                spriteRenderer.color = Color.red;
                hurtTimer = 0.3f;
            }
        }

        public void OnDieHitGroundAnimationFinished() {
            spriteRenderer = deathWeaponSpriteRenderer;
            aliveOutlineSpriteRenderer.enabled = false;
            outlineSpriteRenderer = deathOutlineSpriteRenderer;
            aliveSpriteRenderer.enabled = false;
            deathWeaponSpriteRenderer.enabled = true;
            rigidbody.gravityScale = 0;
            deathReady = true;
        }

        
    }
}
