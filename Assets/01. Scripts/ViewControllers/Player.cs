using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using MikroFramework.Singletons;
using MikroFramework.TimeSystem;
using MikroFramework.Utilities;

using UnityEngine;
using Object = System.Object;

namespace HollowKnight {
    

    public enum PlayerState {
        Normal,
        Hurt,
        Attack,
        Teleport,
        Absorb,
        Die
    }

    public class Player : AbstractMikroController<HollowKnight> {

        public static Player Singleton;

        private IPlayerModel playerModel;

        [SerializeField]
        private PlayerState currentState;
        public PlayerState CurrentState => currentState;

        [Header("Components")]
        private Rigidbody2D rb;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask groundLayer;

        [Header("Movement")]
        
   
        [SerializeField] private float horizontalDirection;

        public bool FaceRight = true;
        
        private bool changingDirection => ((rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f));

        public BindableProperty<float> Speed = new BindableProperty<float>();

        private Trigger2DCheck groundCheck;

        private Animator animator;

        private ITeleportSystem teleportSystem;
        private IAbsorbSystem absorbSystem;

        private IAttackSystem attackSystem;
        public bool OnGround {
            get {
                return (onGround);
            }
        } 


        [Header("Collision Variables")] 

        [SerializeField] private bool onGround = true;

        private void Awake() {
            Singleton = this;
            rb = GetComponent<Rigidbody2D>();
            playerModel = this.GetModel<IPlayerModel>();
            groundCheck = transform.Find("GroundCheck").GetComponent<Trigger2DCheck>();
            animator = GetComponent<Animator>();
            teleportSystem = this.GetSystem<ITeleportSystem>();
            absorbSystem = this.GetSystem<IAbsorbSystem>();
            attackSystem = this.GetSystem<IAttackSystem>();
            currentState = PlayerState.Normal;
            RegisterEvents();

        }

        private void RegisterEvents() {
            this.RegisterEvent<OnTeleportPrepare>(OnTeleportStartPrepare).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnTeleportAppearing>(OnTeleportAppearing).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnTeleportFinished>(OnTeleportFinished).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            this.RegisterEvent<OnAttackStop>(OnAttackStop).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnAttackAiming>(OnAttackAiming).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnAttackStartPrepare>(OnAttackStartPrepare).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnEnemyAbsorbed>(OnAbsorbDone).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            this.RegisterEvent<OnAttackStartPrepare>(OnStartAttack).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnEnemyAbsorbed>(OnEnemyAbsorbed).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnAbsorbInterrupted>(OnAbsorbInterrupted).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnEnemyAbsorbPreparing>(OnAbsorbStart).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            this.RegisterEvent<OnAttackAiming>(OnAiming).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnPlayerDie>(OnDie).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnBossCutSceneComplete>(OnBossCutSceneComplete)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnPlayerRespawned>(OnRespawn).UnRegisterWhenGameObjectDestroyed(gameObject);
            playerModel.CanHurt.RegisterOnValueChaned(OnCanHurtChange).UnRegisterWhenGameObjectDestroyed(gameObject);
            //this.RegisterEvent<OnEnemyAbsorbed>(OnAbsorb).UnRegisterWhenGameObjectDestroyed(gameObject);
            playerModel.Health.RegisterOnValueChaned(OnHealthChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnCanHurtChange(bool old, bool newCanHurt) {
            if (newCanHurt) {
                gameObject.layer = LayerMask.NameToLayer("Player");
                GetComponent<SpriteRenderer>().color = new Color(0.9811321f,
                    0.9811321f, 0.9811321f, 1);
            }
            else {
                gameObject.layer = LayerMask.NameToLayer("PlayerTransparent");
                GetComponent<SpriteRenderer>().color = new Color(0.9811321f,
                    0.9811321f, 0.9811321f, 0.5f);
            }
        }

        private void OnRespawn(OnPlayerRespawned e) {
            playerModel = this.GetModel<IPlayerModel>();
            teleportSystem = this.GetSystem<ITeleportSystem>();
            absorbSystem = this.GetSystem<IAbsorbSystem>();
            attackSystem = this.GetSystem<IAttackSystem>();
            canMove = true;
            playerModel.CanHurt.Value = true;
            frozen = false;
            animator.SetBool("Die",false);
            currentState = PlayerState.Normal;

        }

        private void OnBossCutSceneComplete(OnBossCutSceneComplete obj) {
            FrozePlayer(false);
        }

        private void OnAttackAiming(OnAttackAiming e) {
            if (e.targetPosition.x > transform.position.x)
            {
                transform.DOScaleX(1, 0);
            }

            if (e.targetPosition.x < transform.position.x)
            {
                transform.DOScaleX(-1, 0);
            }

        }

        private void OnAttackStartPrepare(OnAttackStartPrepare e) {
            if (e.targetPosition != null && e.targetPosition != Vector2.zero) {
                if (e.targetPosition.x > transform.position.x)
                {
                    transform.DOScaleX(1, 0);
                }

                if (e.targetPosition.x < transform.position.x)
                {
                    transform.DOScaleX(-1, 0);
                }
            }
        }

        #region Animation Events

        public void PlayWalkSound() {
            AudioManager.Singleton.OnWalk();
        }

        public void PlayRunSound() {
            AudioManager.Singleton.OnRun();
        }

        private void OnEnemyAbsorbed(OnEnemyAbsorbed e)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Absorb")) {
                animator.SetTrigger("Absorb_Finish");
                
            }
        }
        private void OnDie(OnPlayerDie e) {
            if (currentState != PlayerState.Die) {
                currentState = PlayerState.Die;
                animator.SetBool("Die", true);
                this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(1, 0.5f, 30, 120));
                this.SendCommand<TimeSlowCommand>(TimeSlowCommand.Allocate(2f, 0.2f));
            }
           
        }

        private void OnHealthChange(float oldHealth, float newHealth) {
            if (oldHealth > newHealth && newHealth>0) {
                OnPlayerHurt();
            }
        }

        private void OnPlayerHurt() {
            currentState = PlayerState.Hurt;
            attackSystem.StopAttack();
            absorbSystem.AbsorbInterrupt();
            UnMoveable(0.4f, null);
            animator.SetTrigger("Hurt");
        }

        public void OnStopHurt() {
            currentState = PlayerState.Normal;
        }

        private void OnStartAttack(OnAttackStartPrepare e)
        {
            

            animator.SetTrigger("Attack");
        }

        private void OnAttackStop(OnAttackStop e)
        {
            if (currentState == PlayerState.Attack) {
                animator.SetTrigger("Attack_Stop");
            }
           
        }

        private void OnAbsorbStart(OnEnemyAbsorbPreparing e)
        {

            animator.SetTrigger("Absorb");
        }
        private void OnAbsorbDone(OnEnemyAbsorbed obj)
        {
            animator.SetBool("Absorbing", false);
            PlayAbsorbAudio3();
        }
        private void OnAbsorbInterrupted(OnAbsorbInterrupted obj)
        {
            if (currentState == PlayerState.Absorb) {
                animator.SetTrigger("AbsorbInterrupt");
                animator.SetBool("Absorbing", false);
            }
           
        }


        private void OnTeleportFinished(OnTeleportFinished obj)
        {
            //rb.simulated = true;
            rb.gravityScale = 2;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        private void OnTeleportAppearing(OnTeleportAppearing e)
        {
            this.transform.position = e.pos;
            //rb.simulated = false;
            rb.gravityScale = 0;
            gameObject.layer = LayerMask.NameToLayer("Player");
            animator.SetTrigger("Teleport_Appear");
        }

        public void OnTeleportDisappear() {
            gameObject.layer = LayerMask.NameToLayer("PlayerTeleporting");
        }

        private void OnTeleportStartPrepare(OnTeleportPrepare e)
        {
            if (e.targetDest.x > transform.position.x)
            {
                transform.DOScaleX(1, 0);
            }

            if (e.targetDest.x < transform.position.x)
            {
                transform.DOScaleX(-1, 0);
            }

            animator.SetTrigger("Teleport");

            //rb.simulated = false;
            rb.gravityScale = 0;
        }

        private void OnAiming(OnAttackAiming obj)
        {
            if (obj.targetPosition.x > transform.position.x)
            {
                transform.DOScaleX(1, 0);
            }

            if (obj.targetPosition.x < transform.position.x)
            {
                transform.DOScaleX(-1, 0);
            }
        }
        #endregion

        private bool frozen = false;


        public void FrozePlayer(bool isFrozen) {
            frozen = isFrozen;
            if (isFrozen) {
                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.SetTrigger("CutScene");
            }
        }

        private bool canMove = true;
        public void UnMoveable(float lastTime, Action onFinished) {
            canMove = false;
            this.GetSystem<ITimeSystem>().AddDelayTask(lastTime, () => {
                canMove = true;
                onFinished?.Invoke();
            });
        }
        private void Update()
        {
            if (!frozen && GameManager.Singleton.State == GameState.Game) {
                horizontalDirection = GetInput().x;


                if (onGround && GameManager.Singleton.DaggerGet)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        Jump();
                    }
                }

                StateCheck();
                //attack
                AttackControl();
                CheckShiftWeapon();
                CheckDropWeapon();
                

                if (GameManager.Singleton.DaggerGet) {
                    CheckTeleport();
                    CheckAbsorb();
                }
               
                
            }
            AnimationControl();
        }

        private void CheckDropWeapon() {
            if (Input.GetKeyDown(KeyCode.G)) {
                this.SendCommand<DropWeaponCommand>();
            }
        }

        [SerializeField] private List<AudioClip> absorbAudios;
        public void PlayAbsorbAudio1() {
            AudioManager.Singleton.PlayAbsorbAudio(absorbAudios[0],1);
        }
        public void PlayAbsorbAudio2()
        {
            AudioManager.Singleton.PlayAbsorbAudio(absorbAudios[1], 1);
            AudioManager.Singleton.PlayAbsorbAudio(absorbAudios[3], 1);
        }
        public void PlayAbsorbAudio3()
        {
            AudioManager.Singleton.PlayAbsorbAudio(absorbAudios[2], 1);
        }


        private float scrollWheel;
        private float lastScroll;
        private void CheckShiftWeapon() {
            scrollWheel = Input.GetAxis("Mouse ScrollWheel");

            if (lastScroll == 0) {
                if (scrollWheel > 0f) {
                    this.SendCommand<ShiftWeaponCommand>(ShiftWeaponCommand.Allocate(true));
                }

                if (scrollWheel < -0f) {
                    this.SendCommand<ShiftWeaponCommand>(ShiftWeaponCommand.Allocate(false));
                }
            }

            lastScroll = scrollWheel;

        }

        private float absorbMouseHoldTime = 0;
        private void CheckAbsorb() {
            if (currentState == PlayerState.Normal || attackSystem.AttackState == AttackState.Attacking || currentState==PlayerState.Absorb) {
                if (Input.GetMouseButton(1)) { //keep absorbing per frame
                    
                    absorbMouseHoldTime += Time.deltaTime;
                    if (absorbMouseHoldTime >= 0.3f) {
                        bool result = absorbSystem.Absorb(Input.mousePosition);

                        if (result) {
                            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            if (mousePos.x > transform.position.x)
                            {
                                transform.DOScaleX(1, 0);
                            }

                            if (mousePos.x < transform.position.x)
                            {
                                transform.DOScaleX(-1, 0);
                            }
                            animator.SetBool("Absorbing",true);
                        }
                        else {
                            animator.SetBool("Absorbing", false);
                        }
                        

                        attackSystem.StopAttack();
                    }
                    
                }
                else { //mouse up
                    absorbMouseHoldTime = 0;
                    if (currentState == PlayerState.Absorb) {
                        absorbSystem.AbsorbInterrupt();
                       
                    }
                    animator.SetBool("Absorbing", false);
                }

            }
            else {
                animator.SetBool("Absorbing", false);
            }
        }

        private void StateCheck() {

            if (attackSystem.AttackState == AttackState.NotAttacking &&
                teleportSystem.TeleportState == TeleportState.NotTeleporting 
                && absorbSystem.AbsorbState == AbsorbState.NotAbsorbing
                && currentState != PlayerState.Hurt && currentState!=PlayerState.Die) {
                currentState = PlayerState.Normal;
            }else if ((attackSystem.AttackState == AttackState.Attacking || attackSystem.AttackState == AttackState.Preparing)
                && currentState!= PlayerState.Die) {
                currentState = PlayerState.Attack;
            }else if ((teleportSystem.TeleportState == TeleportState.PrepareTeleport ||
                      teleportSystem.TeleportState == TeleportState.TeleportAppearing ||
                      teleportSystem.TeleportState == TeleportState.Teleporting) && currentState!=PlayerState.Die) {
                currentState = PlayerState.Teleport;
            }else if ((absorbSystem.AbsorbState == AbsorbState.Absorbing 
                      || absorbSystem.AbsorbState == AbsorbState.AbsorbPreparing) && currentState!=PlayerState.Die) {
                currentState = PlayerState.Absorb;
            }
        }
        [SerializeField]
        private int remainingTeleportTime = 2;

        [SerializeField] private Trigger2DCheck teleportCheck;
        private void CheckTeleport() {

            if (currentState == PlayerState.Normal || attackSystem.AttackState == AttackState.Attacking 
            || this.GetSystem<ITeleportSystem>().TeleportState == TeleportState.Teleporting) {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (teleportCheck.Triggered) {
                        remainingTeleportTime = 2;
                    }

                    if (this.GetSystem<ITeleportSystem>().TeleportState == TeleportState.NotTeleporting ||
                        this.GetSystem<ITeleportSystem>().TeleportState == TeleportState.TeleportAppearing) {
                        remainingTeleportTime--;
                        if (remainingTeleportTime >= 0)
                        {
                            if (attackSystem.AttackState == AttackState.Attacking) {
                                attackSystem.StopAttack();
                                StartCoroutine(AttackToTeleport(Input.mousePosition));
                            }
                            else {
                                Debug.Log("Teleport 233");
                                teleportSystem.Teleport(Input.mousePosition);
                            }
                        }
                    }
                    else {
                        if (attackSystem.AttackState == AttackState.Attacking)
                        {
                            attackSystem.StopAttack();
                            StartCoroutine(AttackToTeleport(Input.mousePosition));
                        }
                        else
                        {
                            teleportSystem.Teleport(Input.mousePosition);
                        }
                    }
                    

                   
                }
            }
        }

        IEnumerator AttackToTeleport(Vector2 mousePos) {
            yield return new WaitForSeconds(0.6f);
            teleportSystem.Teleport(mousePos);
        }

        private void AttackControl() {
            //stop timer
            attackSystem.CheckAttackPerFrame(currentState);
        }


        private void FixedUpdate()
        {
            if (!frozen) {
                CheckCollisions();
                
                MoveCharacter();
                
                
                if (onGround) {
                    playerModel.ResetRemainingJumpValue();
                }
            }
        }

       
        private void AnimationControl() {
            float horizontalSpeed = Mathf.Abs(rb.velocity.x);
            animator.SetBool("Idle", horizontalSpeed <= 0.5);
            animator.SetBool("Move", horizontalSpeed > 0.5);
            animator.SetFloat("RunSpeed", Mathf.Max(0.4f, horizontalSpeed / playerModel.MaxRunSpeed.Value));
            animator.SetFloat("MoveSpeed", horizontalSpeed / playerModel.MaxRunSpeed.Value);

            if (currentState != PlayerState.Teleport && currentState!=PlayerState.Hurt) {
                if (Input.GetAxis("Horizontal") != 0) {
                    if (rb.velocity.x > 0)
                    {
                        FaceRight = true;
                        transform.localScale = new Vector3(1, 1, 1); // DOScaleX(1, 0);
                    }

                    if (rb.velocity.x < 0)
                    {
                        FaceRight = false;
                        transform.localScale = new Vector3(-1, 1, 1);
                    }
                }
                
            }
            
        }

        private Vector2 GetInput()
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void MoveCharacter() {
            Speed.Value = Mathf.Abs(rb.velocity.magnitude);

            if (horizontalDirection == 0 || !canMove) {
                rb.velocity = new Vector2(rb.velocity.x * playerModel.GroundLinearDrag.Value,
                    rb.velocity.y);
                if (Mathf.Abs(rb.velocity.x) <= 0.1) {
                    rb.velocity = new Vector2(rb.velocity.x * 0.5f,
                        rb.velocity.y);
                }
            }

            if (horizontalDirection != 0 && attackSystem.AttackState == AttackState.Attacking && !Input.GetMouseButton(0)) {
                attackSystem.StopAttack();
            }

            if (currentState == PlayerState.Normal && canMove) {
                bool isWalking = !Input.GetKey(KeyCode.LeftShift);

                float speed = !isWalking ? playerModel.RunSpeed.Value : playerModel.WalkSpeed.Value;
                float maxSpeed = isWalking ? playerModel.MaxWalkSpeed.Value : playerModel.MaxRunSpeed.Value;

                float targetSpeedX = Mathf.Abs((rb.velocity +
                                                new Vector2(horizontalDirection *
                                                            speed * Time.deltaTime, 0)).x);

                if (targetSpeedX <= maxSpeed)
                {
                    if (!onGround)
                    {
                        if (targetSpeedX < 0.5)
                        {
                            AudioManager.Singleton.OnWalkStop();
                            return;
                        }
                    }

                    if (Mathf.Sign(horizontalDirection) == Mathf.Sign(rb.velocity.x)) {
                        rb.velocity = rb.velocity + new Vector2(horizontalDirection * speed * Time.deltaTime, 0);
                    }
                    else {
                        rb.velocity = rb.velocity + new Vector2(4* horizontalDirection * speed * Time.deltaTime, 0);
                    }
                    
                }
                else {
                    float multiplier = Mathf.Sign(rb.velocity.x) * Mathf.Sign(horizontalDirection);
                    rb.velocity = rb.velocity - new Vector2(horizontalDirection * speed * Time.deltaTime, 0) * multiplier;

                }
                
            }
            else {
                AudioManager.Singleton.OnWalkStop();
                if (teleportSystem.TeleportState != TeleportState.NotTeleporting) {
                    rb.velocity = Vector2.zero;
                }

                if (onGround) {
                    if (attackSystem.AttackState != AttackState.NotAttacking ||
                        absorbSystem.AbsorbState != AbsorbState.NotAbsorbing) {
                        rb.velocity = Vector2.zero;
                    }
                }
            }
        }

        


        private void Jump() {
           
            attackSystem.StopAttack();

            if (absorbSystem.AbsorbState != AbsorbState.NotAbsorbing) {
                if (!Input.GetMouseButton(1)) {
                    absorbSystem.AbsorbInterrupt();
                }
                else {
                    return;
                }
            }
            
           

            
            animator.SetTrigger("Jump");

            rb.AddForce(Vector2.up * playerModel.JumpForce.Value, ForceMode2D.Impulse);
           
            
        }

       
        private void CheckCollisions() {

            onGround = groundCheck.Triggered;
        }


    }

}
