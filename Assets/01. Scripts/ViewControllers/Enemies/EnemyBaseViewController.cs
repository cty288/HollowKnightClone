using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight {
    public interface IEnemyViewController {
        public bool Attackable { get; }
        public bool Absorbable { get; }
        public bool CanAttack { get; }

        public bool CanAbsorb { get; }
    }

    public interface IEnemyViewControllerAbsorbable : IEnemyViewControllerAttackable {
        WeaponInfo WeaponInfo { get; }
        /// <summary>
        /// This is only effective if this enemy is absorbable in the configuration model
        /// </summary>
        void OnAbsorbing(float percentage);

        /// <summary>
        /// This is only effective if this enemy is absorbable in the configuration model
        /// </summary>
        void OnAbsorbed();

        /// <summary>
        /// When dropped after absorbed
        /// </summary>
        void OnDropped();
    }

    public interface IEnemyViewControllerAttackable {
        bool IsDie { get; }

        void AttackedByPlayer(int damage);

        /// <summary>
        /// This is only effect if the enemy is attackable
        /// </summary>
        void OnDie();

        GameObject GameObject { get; }
    }


    public abstract class EnemyBaseViewController<T> : AbstractMikroController<HollowKnight>, IEnemyViewController
        where T : EnemyConfigurationItem, new() {

        

        protected T configurationItem;

        protected IEnemyConfigurationModel enemyConfigurationModel;
        [SerializeField]
        protected Transform shakeParent;

        public bool Attackable {
            get { return typeof(T).GetInterface("IAttackable") != null; }
        }

        public bool Absorbable {
            get { return typeof(T).GetInterface("IAbsorbable") != null; }
        }

        public bool CanAttack {
            get { return typeof(T).GetInterface("ICanAttack") != null; }
        }

        public bool CanAbsorb {
            get {
                if (!Absorbable) return false;
                IAbsorbable absorbable = configurationItem as IAbsorbable;
                if (absorbable.Health.Value > 0) {
                    if (!absorbable.CanAbsorbWhenAlive) {
                        return false;
                    }
                }

                if (absorbable.Absorbed.Value) {
                    return false;
                }

                return true;
            }
        }

        protected virtual void Awake() {
            enemyConfigurationModel = this.GetModel<IEnemyConfigurationModel>();
            configurationItem = GetConfigurationItem();
            configurationItem.FSM.OnStateChanged.Register(OnFSMStateChanged);
        }

      
        

        protected virtual void OnDestroy() {
            configurationItem.FSM.OnStateChanged.UnRegister(OnFSMStateChanged);
        }

        protected StateEnum GetCurrentState<StateEnum>() where StateEnum : Enum {
            return (StateEnum) Enum.Parse(typeof(StateEnum), configurationItem.FSM.CurrentState.name);
        }

        protected abstract void OnFSMStateChanged(string prevEvent, string newEvent);

        protected T GetConfigurationItem() {

            return enemyConfigurationModel.GetEnemyConfigurationItemByType<T>();
        }

        protected void TriggerFSM<T>(T eventEnum) where T : Enum {
            configurationItem.FSM.HandleEvent(eventEnum);
        }
        
    }




    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AbstractAbsorbableEnemy<T> : EnemyBaseViewController<T>, IEnemyViewControllerAbsorbable
        where T : EnemyConfigurationItem, new() {
        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        protected Rigidbody2D rigidbody;

        protected TrailRenderer trailRenderer;

        protected WeaponInfo weaponInfo;

        protected IWeaponSystem weaponSystem;

        protected IAbsorbable absorbableConfiguration;

        protected Transform transformer;

        [SerializeField]
        private Sprite[] bulletStateSprites;

        [SerializeField]
        protected Collider2D mouseDetectionTrigger;

        protected override void Awake() {
            base.Awake();
            rigidbody = this.GetComponent<Rigidbody2D>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        protected virtual void Start() {
            this.RegisterEvent<OnEnemyAbsorbPreparing>(OnEnemyPointed)
                .UnRegisterWhenGameObjectDestroyed(this.gameObject);
            this.RegisterEvent<OnEnemyAbsorbed>(OnEnemyAbsorbed).UnRegisterWhenGameObjectDestroyed(this.gameObject);
            this.RegisterEvent<OnEnemyAbsorbing>(OnEnemyAbsorbing).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnAbsorbInterrupted>(OnEnemyAbsorbInterrupted).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnEnemyAbsorbPreparing>(OnEnemyAbsorbStartPrepare).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnWeaponDropped>(OnWeaponDropped).UnRegisterWhenGameObjectDestroyed(gameObject);

            (configurationItem as IAttackable).Health.RegisterOnValueChaned(OnHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            weaponSystem = this.GetSystem<IWeaponSystem>();
            absorbableConfiguration = configurationItem as IAbsorbable;

            weaponInfo = weaponSystem.GetWeaponFromConfig(absorbableConfiguration.WeaponName);

            weaponInfo.BulletCount.RegisterOnValueChaned(OnBulletCountChange);
            OnBulletCountChange(weaponInfo.BulletCount.Value,weaponInfo.BulletCount.Value);

        }

        protected void OnBulletCountChange(int oldBullet, int newBullet) {
            if (newBullet > 0) {
                int spriteIndex = weaponInfo.MaxBulletCount - newBullet;
                spriteRenderer.sprite = bulletStateSprites[spriteIndex];
            }
            else {
                if (transformer) {
                    transformer.SetParent(null);
                    Destroy(transformer.gameObject,1f);
                }
                Destroy(gameObject);
            }
        }

        protected void AddTransformAsTransformer() {
            if (transformer) {
                Destroy(transformer.gameObject);
            }

            GameObject gameObject = new GameObject(name + " Parent");
            transformer = gameObject.transform;

            transformer.SetParent(transform);
            transformer.localPosition = Vector3.zero;
            
            transformer.SetParent(null);

            transform.SetParent(transformer);
        }

        protected void AddRectTransformAsTransformer() {
            if (transformer) {
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

        private void OnWeaponDropped(OnWeaponDropped e) {
            if (e.DroppedWeapon == weaponInfo) {
                if (IsDie) {
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
                    rigidbody.AddForce(new Vector2(5 * right,3), ForceMode2D.Impulse);

                    rigidbody.gravityScale = 1;
                    trailRenderer.enabled = false;

                   
                    OnDropped();
                }
            }
        }

        private void OnEnemyAbsorbStartPrepare(OnEnemyAbsorbPreparing e) {
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject) {
                OnStartPrepareAbsorb();
            }
        }

        private void OnEnemyAbsorbInterrupted(OnAbsorbInterrupted e) {
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject) {
               OnAbsorbInterrupt();
            }
        }

        private void OnHealthChanged(int old, int newHealth)
        {
            if (newHealth < old)
            {
                OnAttacked(newHealth-old);
            }

            if (newHealth <= 0)
            {
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
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject)
            {
                if (CanAbsorb) {
                    spriteRenderer.color = new Color(1, 1-e.absorbPercentage, 1-e.absorbPercentage);
                    OnAbsorbing(e.absorbPercentage);
                }
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

                    //this.gameObject.AddComponent<RectTransform>();
                    AddRectTransformAsTransformer();

                    trailRenderer.enabled = true;
                    OnAbsorbed();

                    this.SendCommand<AddEnemyViewControllerToLayoutCircleCommand>(AddEnemyViewControllerToLayoutCircleCommand.Allocate(this, transformer.gameObject));
                }
            }
        }

        public bool IsDie {
            get {
                return absorbableConfiguration.Health.Value <= 0;
            }
        }
        

        public void AttackedByPlayer(int damage) {
            absorbableConfiguration.Health.Value -= damage;
        }

        public virtual void OnDie() {
            rigidbody.velocity = Vector2.zero;
            rigidbody.gravityScale = 0;
        }

        public GameObject GameObject {
            get {
                return this.gameObject;
            }
        }

        public WeaponInfo WeaponInfo {
            get {
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

        public virtual void OnAttacked(int damage) { }

        public virtual void OnAbsorbInterrupt() { }
    }
}

