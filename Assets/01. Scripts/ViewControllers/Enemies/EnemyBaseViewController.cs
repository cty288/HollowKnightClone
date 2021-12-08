using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Examples.ServiceLocator;
using UnityEngine;
using FSM = MikroFramework.FSM.FSM;

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
        public IAttackable Attackable { get; }
        bool IsDie { get; }

        void AttackedByPlayer(int damage);

        /// <summary>
        /// This is only effect if the enemy is attackable
        /// </summary>
        void OnDie();

        GameObject GameObject { get; }
    }

    public interface IEnemyViewControllerCanAttack<AttackStageEnum> where AttackStageEnum:Enum{
         AttackStageEnum CurrentFSMStage { get; } 
         ICanAttack CanAttackConfig { get; }

        bool IsAttacking { get; }

        void HurtPlayerWithCurrentAttackStage();

        float GetCurrentAttackRate();

        void HurtPlayerNoMatterWhatAttackStage(float damage);

        void OnAttackingStage(Enum attackStage);

        void OnFSMStage(AttackStageEnum currentStage);
    }


    public abstract class EnemyBaseViewController<T> : AbstractMikroController<HollowKnight>, IEnemyViewController
        where T : EnemyConfigurationItem, new() {

        protected FSM FSM {
            get {
                return configurationItem.FSM;
            }
        }

        protected T configurationItem;

        protected IEnemyConfigurationModel enemyConfigurationModel;
        [SerializeField]
        protected Transform shakeParent;

        public bool Attackable {
            get {
                return typeof(T).GetInterface("IAttackable") != null; 
                
            }
        }

        public bool Absorbable {
            get { return typeof(T).GetInterface("IAbsorbable") != null; }
        }

        public bool CanAttack {
            get { return typeof(T).GetInterface("ICanAttack") != null; }
        }
        protected bool deathReady = true;
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

                if (!deathReady) {
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

        [SerializeField]
        protected SpriteRenderer outlineSpriteRenderer;

        protected Rigidbody2D rigidbody;

        protected TrailRenderer trailRenderer;

        protected WeaponInfo weaponInfo;

        protected IWeaponSystem weaponSystem;

        protected IAbsorbable absorbableConfiguration;

        protected Transform transformer;
        

        public IAttackable Attackable {
            get {
                return (configurationItem) as IAttackable;
            }
        }

        [SerializeField]
        protected Sprite[] bulletStateSprites;

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
            this.RegisterEvent<OnAttackAiming>(OnAiming).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            (configurationItem as IAttackable).Health.RegisterOnValueChaned(OnHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            weaponSystem = this.GetSystem<IWeaponSystem>();
            absorbableConfiguration = configurationItem as IAbsorbable;

            weaponInfo = weaponSystem.GetWeaponFromConfig(absorbableConfiguration.WeaponName);

            weaponInfo.BulletCount.RegisterOnValueChaned(OnBulletCountChange).UnRegisterWhenGameObjectDestroyed(gameObject);
            //OnBulletCountChange(weaponInfo.BulletCount.Value,weaponInfo.BulletCount.Value);

        }

        private void OnAiming(OnAttackAiming e) {
            if (e.Target == this.gameObject) {
                outlineSpriteRenderer.enabled = true;
            }
        }

        protected void OnBulletCountChange(int oldBullet, int newBullet) {
            if (oldBullet < newBullet) {
                OnBulletShot(newBullet - oldBullet);
            }

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

                    gameObject.layer = LayerMask.NameToLayer("Enemy");
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
               outlineSpriteRenderer.enabled = false;
            }
            else {
              
            }
        }

        protected virtual void OnMouseOver() {
           
            IAbsorbSystem absorbSystem = this.GetSystem<IAbsorbSystem>();
            IAttackSystem attackSystem = this.GetSystem<IAttackSystem>();

            if (absorbSystem.AbsorbState != AbsorbState.NotAbsorbing ) {
                if (absorbSystem.AbsorbingGameObject != this.gameObject)
                {
                    return;
                }
            }

            if (attackSystem.AttackState != AttackState.NotAttacking) {
                return;
            }

            if (absorbableConfiguration.Absorbed.Value) {
                return;
            }
            outlineSpriteRenderer.enabled = true;
        }


        

        protected virtual void OnMouseExit() {
            if (this.GetSystem<IAbsorbSystem>().AbsorbState == AbsorbState.NotAbsorbing) {
                outlineSpriteRenderer.enabled = false;
            }
            
        }


        protected void Update() {
            
        }

        private void OnHealthChanged(float old, float newHealth)
        {
            if (newHealth < old)
            {
                Debug.Log($"Attacked, add to charge {old - newHealth}");
                this.SendCommand<ChargeUltCommand>(ChargeUltCommand.Allocate(old - newHealth));
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

        private void OnEnemyAbsorbing(OnEnemyAbsorbing e) {
            rigidbody.gravityScale = 0;
            if (e.absorbedEnemy && e.absorbedEnemy == gameObject)
            {
                if (CanAbsorb) {
                    outlineSpriteRenderer.enabled = true;
                    spriteRenderer.color = new Color(1, 1-e.absorbPercentage, 1-e.absorbPercentage);
                    OnAbsorbing(e.absorbPercentage);
                    outlineSpriteRenderer.enabled = true;
                }
            }
            else {
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
                    
                    if (!absorbableConfiguration.HealthRestored) {
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

   
        public bool IsDie {
            get {
                return absorbableConfiguration.Health.Value <= 0;
            }
        }
        

        public void AttackedByPlayer(int damage) {
            absorbableConfiguration.Health.Value -= damage;
        }

        //instant
        public virtual void OnDie() {
            rigidbody.velocity = Vector2.zero;
            StartCoroutine(SetGravityScaleZero());
        }

        private IEnumerator SetGravityScaleZero() {
            yield return new WaitForSeconds(0.5f);
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

        public virtual void OnAttacked(float damage) { }

        public virtual void OnAbsorbInterrupt() { }

        public virtual void OnBulletShot(int number) { }
    }

    public abstract class AbstractCanAttackEnemy<T, AttackStageEnum> : EnemyBaseViewController<T>, 
        IEnemyViewControllerAttackable, IEnemyViewControllerCanAttack<AttackStageEnum>
        where T : EnemyConfigurationItem, new() where AttackStageEnum : Enum {

        public IAttackable Attackable {
            get {
                return (configurationItem) as IAttackable;
            }
        }


        public bool IsDie
        {
            get {
                return Attackable.Health.Value <= 0;
            }
        }

        public void AttackedByPlayer(int damage) {
            Attackable.Attack(damage);
        }

        public virtual void OnDie() { }

        public GameObject GameObject {
            get {
                return this.gameObject;
            }
        }


        public AttackStageEnum CurrentFSMStage {
            get {
                return (AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name, true);
            }
        }


        public ICanAttack CanAttackConfig {
            get {
                return (configurationItem) as ICanAttack;
            }
        }


        public bool IsAttacking {
            get {
                foreach (Enum attackStageName in CanAttackConfig.AttackStageNames) {
                    if (attackStageName.ToString() == FSM.CurrentState.name) {
                        return true;
                    }
                }
                return false;
            }
        }

        public void HurtPlayerWithCurrentAttackStage() {
            if (IsAttacking)
            {
                this.GetModel<IPlayerModel>().ChangeHealth(-CanAttackConfig.AttackSkillDamages
                    [(AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name)]);
            }
        }

        public float GetCurrentAttackRate() {
            if (IsAttacking) {
                return (CanAttackConfig.AttackFreqs
                    [(AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name)]);
            }

            return 0;
        }

        public void HurtPlayerNoMatterWhatAttackStage(float damage) {
            this.GetModel<IPlayerModel>().ChangeHealth(-damage);
        }

        public abstract void OnAttackingStage(Enum attackStage);


        public abstract void OnFSMStage(AttackStageEnum currentStage);

    }



    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AbstractAbsorbableCanAttackEnemy<T, AttackStageEnum> : AbstractAbsorbableEnemy<T>,
        IEnemyViewControllerCanAttack<AttackStageEnum> where T : EnemyConfigurationItem, new() 
    where AttackStageEnum: Enum {
        [SerializeField]
        protected Transform eyePosition;

        [SerializeField] 
        protected float viewDistance = 10;

        [SerializeField] 
        protected LayerMask eyeDetectLayers;

        [SerializeField] 
        protected Transform bulletSpawnPosition;

        [SerializeField] protected GameObject bulletPrefab;

        protected bool FaceLeft {
            get {
                return faceLeft;
            }
            set {
                faceLeft = value;
            }
        }

        private bool faceLeft;
        public AttackStageEnum CurrentFSMStage {
            get {
                return (AttackStageEnum) Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name, true);
            }
        }
        public ICanAttack CanAttackConfig {
            get {
                return (configurationItem) as ICanAttack;
            }
        }

        public bool IsAttacking {
            get {
                foreach (Enum attackStageName in CanAttackConfig.AttackStageNames) {
                    if (attackStageName.ToString() == FSM.CurrentState.name) {
                        return true;
                    }
                }

                return false;
            }
        }

        protected void Update() {
            base.Update();

            foreach (Enum attackStageName in CanAttackConfig.AttackStageNames) {
                if (attackStageName.ToString() == FSM.CurrentState.name) {
                    OnAttackingStage(attackStageName);
                    break;
                }
            }

            float direction = FaceLeft ? -1 : 1;

            if (eyePosition) {
                RaycastHit2D hit = Physics2D.Raycast(eyePosition.position, eyePosition.right * direction, viewDistance,
                    eyeDetectLayers, -50f, 50f);

                if (hit.collider)
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        if (Player.Singleton.CurrentState != PlayerState.Die)
                        {
                            OnSeePlayer();
                        }
                        else
                        {
                            OnNotSeePlayer();
                        }
                    }
                    else
                    {
                        OnNotSeePlayer();
                    }
                }
                else
                {
                    OnNotSeePlayer();
                }
            }
           

            OnFSMStage(CurrentFSMStage);
        }


        protected abstract void OnSeePlayer();

        protected void TriggerEvent(Enum eventEnum) {
            FSM.HandleEvent(eventEnum);
        }

        /// <summary>
        /// Damage = damage of current attack stage. Should only be called when in attack stage
        /// </summary>
        public void HurtPlayerWithCurrentAttackStage() {
            if (IsAttacking) {
                this.GetModel<IPlayerModel>().ChangeHealth(-CanAttackConfig.AttackSkillDamages
                    [(AttackStageEnum) Enum.Parse(typeof(AttackStageEnum),FSM.CurrentState.name)]);
            }
        }

        public float GetCurrentAttackRate() {
            if (IsAttacking)
            {
                return (CanAttackConfig.AttackFreqs
                    [(AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name)]);
            }

            return 0;
        }

        public void HurtPlayerNoMatterWhatAttackStage(float damage) {
            this.GetModel<IPlayerModel>().ChangeHealth(-damage);
        }
        private Tween moveTween;

        public override void OnAbsorbInterrupt() {
            StopAllCoroutines();
            Debug.Log("Mouse Interrupt");
            if (moveTween != null)
            {
                moveTween.Kill();
            }
            spriteRenderer.gameObject.transform.DOLocalMoveY(0, 0.3f);
        }

        public override void OnStartPrepareAbsorb() {
            base.OnStartPrepareAbsorb();
            StartCoroutine(Float());
        }


        public override void OnAbsorbed()
        {
            base.OnAbsorbed();
            if (moveTween != null)
            {
                moveTween.Kill();
            }
        }


        private IEnumerator Float()
        {
            yield return new WaitForSeconds(0.83f);

            spriteRenderer.transform.parent.DOShakePosition(1.67f, 0.2f, 20, 100);

            moveTween = spriteRenderer.gameObject.transform.DOMoveY(spriteRenderer.gameObject.transform.position.y + 3, 1.67f);
        }


        public abstract void OnAttackingStage(Enum attackStage);

        public abstract void OnFSMStage(AttackStageEnum currentStage);
        protected override void OnFSMStateChanged(string prevEvent, string newEvent) {
            
        }

        protected abstract void OnNotSeePlayer();
    }
}

