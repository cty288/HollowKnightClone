using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AbstractAbsorbableEnemy<T> : EnemyBaseViewController<T>, IEnemyViewControllerAbsorbable
        where T : EnemyConfigurationItem, new() {
       

        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        [SerializeField]
        protected SpriteRenderer outlineSpriteRenderer;

        protected Rigidbody2D rigidbody;

        protected TrailRenderer trailRenderer;

        protected WeaponInfo weaponInfo;

        protected IWeaponSystem weaponSystem;

        protected IAbsorbable absorbableConfiguration;

        protected Transform transformer;


        public IAttackable Attackable
        {
            get
            {
                return (configurationItem) as IAttackable;
            }
        }

        [SerializeField]
        protected Sprite[] bulletStateSprites;

        [SerializeField]
        protected Collider2D mouseDetectionTrigger;

        protected override void Awake()
        {
            base.Awake();
            rigidbody = this.GetComponent<Rigidbody2D>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        protected virtual void Start()
        {
            this.RegisterEvent<OnEnemyAbsorbPreparing>(OnEnemyPointed)
                .UnRegisterWhenGameObjectDestroyed(this.gameObject);
            this.RegisterEvent<OnEnemyAbsorbed>(OnEnemyAbsorbed).UnRegisterWhenGameObjectDestroyed(this.gameObject);
            this.RegisterEvent<OnEnemyAbsorbing>(OnEnemyAbsorbing).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnAbsorbInterrupted>(OnEnemyAbsorbInterrupted).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnEnemyAbsorbPreparing>(OnEnemyAbsorbStartPrepare).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnWeaponDropped>(OnWeaponDropped).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnAttackAiming>(OnAiming).UnRegisterWhenGameObjectDestroyed(gameObject);
            //this.RegisterEvent<OnPlayerRespawned>(OnRespawned).UnRegisterWhenGameObjectDestroyed(gameObject);
            (configurationItem as IAttackable).Health.RegisterOnValueChaned(OnHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            weaponSystem = this.GetSystem<IWeaponSystem>();
            absorbableConfiguration = configurationItem as IAbsorbable;

            weaponInfo = weaponSystem.GetWeaponFromConfig(absorbableConfiguration.WeaponName);

            weaponInfo.BulletCount.RegisterOnValueChaned(OnBulletCountChange).UnRegisterWhenGameObjectDestroyed(gameObject);
            //OnBulletCountChange(weaponInfo.BulletCount.Value,weaponInfo.BulletCount.Value);

            if (BornToBeDead) {
                Attackable.Kill();
                
            }
        }

        private void OnRespawned(OnPlayerRespawned obj) {
            if (!IsDie)
            {
                Attackable.Restore();
            }
        }

        private void OnAiming(OnAttackAiming e)
        {
            if (e.Target == this.gameObject)
            {
                outlineSpriteRenderer.enabled = true;
            }
        }

        protected void OnBulletCountChange(int oldBullet, int newBullet)
        {
            if (oldBullet < newBullet)
            {
                OnBulletShot(newBullet - oldBullet);
            }

            if (newBullet > 0)
            {
                int spriteIndex = weaponInfo.MaxBulletCount - newBullet;
                spriteRenderer.sprite = bulletStateSprites[spriteIndex];
            }
            else
            {
                if (transformer)
                {
                    transformer.SetParent(null);
                    Destroy(transformer.gameObject, 1f);
                }
                Destroy(gameObject);
            }
        }

        protected void AddTransformAsTransformer()
        {
            if (transformer)
            {
                Destroy(transformer.gameObject);
            }

            GameObject gameObject = new GameObject(name + " Parent");
            transformer = gameObject.transform;

            transformer.SetParent(transform);
            transformer.localPosition = Vector3.zero;

            transformer.SetParent(null);

            transform.SetParent(transformer);
        }

        protected void AddRectTransformAsTransformer()
        {
            if (transformer)
            {
                Destroy(transformer.gameObject);
            }

            GameObject go = new GameObject(name + " Parent");

            go.gameObject.AddComponent<RectTransform>();

            transformer = go.transform;
            go.GetComponent<RectTransform>().sizeDelta = Vector2.one;

            transformer.SetParent(this.transform);
            transformer.localPosition = Vector2.zero;
            //transformer.DOScaleX(1, 0);
            transformer.SetParent(null);

            transform.SetParent(transformer);
        }

        private void OnWeaponDropped(OnWeaponDropped e)
        {
            if (e.DroppedWeapon == weaponInfo)
            {
                if (IsDie)
                {
                    //dropped to ground
                    absorbableConfiguration.Drop();

                    mouseDetectionTrigger.enabled = true;


                    // this.gameObject.;
                    this.transform.SetParent(null);

                    if (transformer)
                    {
                        transformer.SetParent(null);
                        Destroy(transformer.gameObject, 2);
                    }


                    float right = Player.Singleton.FaceRight ? 1 : -1;
                    rigidbody.AddForce(new Vector2(5 * right, 3), ForceMode2D.Impulse);

                    rigidbody.gravityScale = 1;
                    trailRenderer.enabled = false;

                    gameObject.layer = LayerMask.NameToLayer("Enemy");
                    OnDropped();
                }
            }
        }

        private void OnEnemyAbsorbStartPrepare(OnEnemyAbsorbPreparing e)
        {
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject)
            {
                OnStartPrepareAbsorb();
            }
        }

        private void OnEnemyAbsorbInterrupted(OnAbsorbInterrupted e)
        {
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject)
            {
                OnAbsorbInterrupt();
                outlineSpriteRenderer.enabled = false;
            }
            else
            {

            }
        }

        protected virtual void OnMouseOver()
        {

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




        protected virtual void OnMouseExit()
        {
            if (this.GetSystem<IAbsorbSystem>().AbsorbState == AbsorbState.NotAbsorbing)
            {
                outlineSpriteRenderer.enabled = false;
            }

        }


        protected void Update()
        {

        }

        private void OnHealthChanged(float old, float newHealth)
        {
            if (newHealth < old)
            {
                Debug.Log($"Attacked, add to charge {old - newHealth}");
                if (!BornToBeDead) {
                    this.SendCommand<ChargeUltCommand>(ChargeUltCommand.Allocate(old - newHealth));
                }
                
                OnAttacked(old - newHealth);
            }

            if (newHealth <= 0)
            {
                Debug.Log("Die");
                OnDie();
            }
        }

        private void OnEnemyPointed(OnEnemyAbsorbPreparing e)
        {
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject)
            {
                if (CanAbsorb)
                {

                    if (absorbableConfiguration.Health.Value > 0)
                    {
                        this.SendCommand<KillEnemyCommand>(KillEnemyCommand.Allocate(configurationItem as IAttackable));
                    }
                }
            }
        }

        private void OnEnemyAbsorbing(OnEnemyAbsorbing e)
        {
            rigidbody.gravityScale = 0;
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject)
            {
                if (CanAbsorb)
                {
                    outlineSpriteRenderer.enabled = true;
                    spriteRenderer.color = new Color(1, 1 - e.absorbPercentage, 1 - e.absorbPercentage);
                    OnAbsorbing(e.absorbPercentage);
                    outlineSpriteRenderer.enabled = true;
                }
            }
            else
            {
                outlineSpriteRenderer.enabled = false;
            }
        }



        private void OnEnemyAbsorbed(OnEnemyAbsorbed e)
        {
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject)
            {
                if (CanAbsorb)
                {
                    rigidbody.velocity = Vector2.zero;
                    rigidbody.gravityScale = 0;

                    if (!absorbableConfiguration.HealthRestored)
                    {
                        this.GetModel<IPlayerModel>().ChangeHealth(absorbableConfiguration.HealthRestoreWhenAbsorb);
                    }

                    absorbableConfiguration.Absorb();
                    mouseDetectionTrigger.enabled = false;
                    spriteRenderer.color = Color.white;

                    //arrange parent and children position
                    spriteRenderer.gameObject.transform.SetParent(null);
                    this.transform.SetParent(spriteRenderer.gameObject.transform);
                    this.transform.localPosition = Vector2.zero;
                    this.transform.SetParent(null);
                    spriteRenderer.gameObject.transform.SetParent(shakeParent);

                    transform.DOScaleX(1, 0);
                    outlineSpriteRenderer.enabled = false;
                    //this.gameObject.AddComponent<RectTransform>();
                    AddRectTransformAsTransformer();

                    trailRenderer.enabled = true;
                    gameObject.layer = LayerMask.NameToLayer("AbsorbedEnemy");


                    //restore health

                    OnAbsorbed();

                    this.SendCommand<AddEnemyViewControllerToLayoutCircleCommand>(AddEnemyViewControllerToLayoutCircleCommand.Allocate(this, transformer.gameObject));
                }
            }
        }


        public bool IsDie
        {
            get
            {
                return absorbableConfiguration.Health.Value <= 0;
            }
        }


        public void AttackedByPlayer(int damage)
        {
            absorbableConfiguration.Health.Value -= damage;
        }

        public bool BornToBeDead { get; set; } = false;

        //instant
        public virtual void OnDie()
        {
            rigidbody.velocity = Vector2.zero;
            StartCoroutine(SetGravityScaleZero());
        }

        private IEnumerator SetGravityScaleZero()
        {
            yield return new WaitForSeconds(0.5f);
            rigidbody.gravityScale = 0;
        }

        public GameObject GameObject
        {
            get
            {
                return this.gameObject;
            }
        }

        public WeaponInfo WeaponInfo
        {
            get
            {
                return weaponInfo;
            }
        }

        /// <summary>
        /// Keep calling while absorbing
        /// </summary>
        public virtual void OnAbsorbing(float percentage) { }

        public virtual void OnAbsorbed() { }

        public virtual void OnDropped() { }

        public virtual void OnStartPrepareAbsorb() { }

        public virtual void OnAttacked(float damage) { }

        public virtual void OnAbsorbInterrupt() { }

        public virtual void OnBulletShot(int number) { }
    }
}
