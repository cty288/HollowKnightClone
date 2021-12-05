using System;
using System.Collections;
using System.Collections.Generic;
using CodiceApp.EventTracking;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.Event;
using MikroFramework.Singletons;
using MikroFramework.Utilities;
using UnityEditor.UIElements;
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

    public class Player : AbstractMikroController<HollowKnight>, ISingleton {
        public static Player Singleton {
            get {
                return SingletonProperty<Player>.Singleton;
            }
        }
        private IPlayerModel playerModel;

        [SerializeField]
        private PlayerState currentState;

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

        private void Awake()
        {
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
            this.RegisterEvent<OnAttackStartPrepare>(OnStartAttack).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnAbsorbInterrupted>(OnAbsorbInterrupted).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnEnemyAbsorbPreparing>(OnAbsorbStart).UnRegisterWhenGameObjectDestroyed(gameObject);
            
            this.RegisterEvent<OnAttackAiming>(OnAiming).UnRegisterWhenGameObjectDestroyed(gameObject);
            //this.RegisterEvent<OnEnemyAbsorbed>(OnAbsorb).UnRegisterWhenGameObjectDestroyed(gameObject);
            playerModel.Health.RegisterOnValueChaned(OnHealthChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        }



        #region Animation Events
        private void OnHealthChange(float oldHealth, float newHealth) {
            if (oldHealth > newHealth) {
                OnPlayerHurt();
            }
        }

        private void OnPlayerHurt() {
            currentState = PlayerState.Hurt;
            attackSystem.StopAttack();
           
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
            animator.SetTrigger("Attack_Stop");
        }

        private void OnAbsorbStart(OnEnemyAbsorbPreparing e)
        {

            animator.SetTrigger("Absorb");
        }

        private void OnAbsorbInterrupted(OnAbsorbInterrupted obj)
        {
          
           animator.SetTrigger("AbsorbInterrupt");
        }


        private void OnTeleportFinished(OnTeleportFinished obj)
        {
            rb.simulated = true;
        }

        private void OnTeleportAppearing(OnTeleportAppearing e)
        {
            this.transform.position = e.pos;
            rb.simulated = false;
            animator.SetTrigger("Teleport_Appear");
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
            rb.simulated = false;
        }

        private void OnAiming(OnAttackAiming obj)
        {
            if (obj.Target.transform.position.x > transform.position.x)
            {
                transform.DOScaleX(1, 0);
            }

            if (obj.Target.transform.position.x < transform.position.x)
            {
                transform.DOScaleX(-1, 0);
            }
        }
        #endregion



        private void Update()
        {
            horizontalDirection = GetInput().x;
            if (onGround) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Jump();
                }
            }

            StateCheck();
            
            //attack
            AttackControl();
            

            CheckShiftWeapon();
            CheckDropWeapon();
            AnimationControl();
            CheckTeleport();
            CheckAbsorb();
        }

        private void CheckDropWeapon() {
            if (Input.GetKeyDown(KeyCode.Q)) {
                this.SendCommand<DropWeaponCommand>();
            }
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
                        }
                        

                        attackSystem.StopAttack();
                    }
                    
                }
                else { //mouse up
                    absorbMouseHoldTime = 0;
                    if (currentState == PlayerState.Absorb) {
                        absorbSystem.AbsorbInterrupt();
                    }
                }

            }
        }

        private void StateCheck() {
            if (attackSystem.AttackState == AttackState.NotAttacking &&
                teleportSystem.TeleportState == TeleportState.NotTeleporting 
                && absorbSystem.AbsorbState == AbsorbState.NotAbsorbing
                && currentState != PlayerState.Hurt) {
                currentState = PlayerState.Normal;
            }else if (attackSystem.AttackState == AttackState.Attacking || attackSystem.AttackState == AttackState.Preparing) {
                currentState = PlayerState.Attack;
            }else if (teleportSystem.TeleportState == TeleportState.PrepareTeleport ||
                      teleportSystem.TeleportState == TeleportState.TeleportAppearing ||
                      teleportSystem.TeleportState == TeleportState.Teleporting) {
                currentState = PlayerState.Teleport;
            }else if (absorbSystem.AbsorbState == AbsorbState.Absorbing 
                      || absorbSystem.AbsorbState == AbsorbState.AbsorbPreparing) {
                currentState = PlayerState.Absorb;
            }
        }

        private void CheckTeleport() {

            if (currentState == PlayerState.Normal || attackSystem.AttackState == AttackState.Attacking 
            || this.GetSystem<ITeleportSystem>().TeleportState == TeleportState.Teleporting) {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    if (attackSystem.AttackState == AttackState.Attacking) {
                        attackSystem.StopAttack();
                        StartCoroutine(AttackToTeleport(Input.mousePosition));
                    }
                    else {
                        teleportSystem.Teleport(Input.mousePosition);
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
            CheckCollisions();
            MoveCharacter();
            if (onGround) {
                playerModel.ResetRemainingJumpValue();
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

            if (horizontalDirection == 0) {
                rb.velocity = new Vector2(rb.velocity.x * playerModel.GroundLinearDrag.Value,
                    rb.velocity.y);
            }

            if (horizontalDirection != 0 && attackSystem.AttackState == AttackState.Attacking && !Input.GetMouseButton(0)) {
                attackSystem.StopAttack();
            }

            if (currentState == PlayerState.Normal) {
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


        void ISingleton.OnSingletonInit() {
            
        }
    }

}
