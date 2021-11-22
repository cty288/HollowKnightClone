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
    public enum AttackState {
        NotAttacking,
        Preparing,
        Attacking
    }

    public enum PlayerState {
        Normal,
        Attack,
        Teleport
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


        private bool changingDirection => ((rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f));

        public BindableProperty<float> Speed = new BindableProperty<float>();

        private Trigger2DCheck groundCheck;

        private Animator animator;

        private ITeleportSystem teleportSystem;
        private bool canJump {
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
            currentState = PlayerState.Normal;
            RegisterEvents();

        }

        private void RegisterEvents() {
            this.RegisterEvent<OnTeleportPrepare>(OnTeleportStartPrepare).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnTeleportAppearing>(OnTeleportAppearing).UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<OnTeleportFinished>(OnTeleportFinished).UnRegisterWhenGameObjectDestroyed(gameObject);
            
        }

        private void OnTeleportFinished(OnTeleportFinished obj) {
            rb.simulated = true;
        }

        private void OnTeleportAppearing(OnTeleportAppearing e) {
            this.transform.position = e.pos;
            rb.simulated = false;
            animator.SetTrigger("Teleport_Appear");
        }

        private void OnTeleportStartPrepare(OnTeleportPrepare e) {
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

        [SerializeField]
        private AttackState attackState = AttackState.NotAttacking;
        
        private float attackTimer = 0;
        private float attackStopTimer = 0;
        [SerializeField] 
        private float chargeAttackTimeThreshold = 0.2f;

        [SerializeField]
        private float attackStopTimeThreshold = 1f;

        [SerializeField] private float attackPrepareTime = 0.5f;

        private void Update()
        {
            horizontalDirection = GetInput().x;
            if (canJump) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Jump();
                }
            }

            StateCheck();
            if (onGround) {
                //attack
                AttackControl();
            }

            AnimationControl();
            CheckTeleport();
        }

        private void StateCheck() {
            if (attackState == AttackState.NotAttacking &&
                teleportSystem.TeleportState == TeleportState.NotTeleporting) {
                currentState = PlayerState.Normal;
            }else if (attackState == AttackState.Attacking || attackState == AttackState.Preparing) {
                currentState = PlayerState.Attack;
            }else if (teleportSystem.TeleportState == TeleportState.PrepareTeleport ||
                      teleportSystem.TeleportState == TeleportState.TeleportAppearing ||
                      teleportSystem.TeleportState == TeleportState.Teleporting) {
                currentState = PlayerState.Teleport;
            }
        }

        private void CheckTeleport() {

            if (currentState == PlayerState.Normal || attackState == AttackState.Attacking 
            || this.GetSystem<ITeleportSystem>().TeleportState == TeleportState.Teleporting) {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    if (attackState == AttackState.Attacking) {
                        StopAttack();
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
            if (attackState == AttackState.Attacking)
            {
                attackStopTimer += Time.deltaTime;
                if (attackStopTimer >= attackStopTimeThreshold)
                {
                    StopAttack();
                }
            }

            if (attackState == AttackState.NotAttacking && currentState == PlayerState.Normal)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PrepareAttack();
                }
            }


            if (attackState == AttackState.Preparing || attackState == AttackState.Attacking)
            {
                if (attackState == AttackState.Preparing)
                {
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= attackPrepareTime)
                    {
                        attackTimer = 0;
                        attackState = AttackState.Attacking;
                        NormalAttack();
                        Debug.Log("Normal Attack");
                    }
                }

                if (attackState == AttackState.Attacking)
                {
                    if (Input.GetMouseButton(0))
                    { //charge
                        attackStopTimer = 0;
                        attackTimer += Time.deltaTime;
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (attackTimer <= chargeAttackTimeThreshold)
                        {
                            //normal attack
                            NormalAttack();
                            Debug.Log("Normal Attack");
                        }
                        else
                        {
                            //charge attack
                            Debug.Log("Charge Attack");
                        }

                        attackTimer = 0;
                    }
                }
            }


        }

        private void NormalAttack() {

        }

       

        private void FixedUpdate()
        {
            CheckCollisions();
           
            MoveCharacter();
                if (onGround)
            {
                playerModel.ResetRemainingJumpValue();
            }
        }

       
        private void AnimationControl() {
            float horizontalSpeed = Mathf.Abs(rb.velocity.x);
            animator.SetBool("Idle", horizontalSpeed <= 1);
            animator.SetBool("Move", horizontalSpeed > 1);
            animator.SetFloat("RunSpeed", Mathf.Max(0.4f, horizontalSpeed / playerModel.MaxRunSpeed.Value));
            animator.SetFloat("MoveSpeed", horizontalSpeed / playerModel.MaxRunSpeed.Value);

            if (currentState != PlayerState.Teleport) {
                if (rb.velocity.x > 0)
                {
                    transform.DOScaleX(1, 0);
                }

                if (rb.velocity.x < 0)
                {
                    transform.DOScaleX(-1, 0);
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

            if (horizontalDirection != 0 && attackState == AttackState.Attacking && !Input.GetMouseButton(0)) {
                StopAttack();
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
                    rb.velocity = rb.velocity + new Vector2(horizontalDirection * speed * Time.deltaTime, 0);
                }
                else {
                    rb.velocity = rb.velocity - new Vector2(horizontalDirection * speed * Time.deltaTime, 0);
                }
            }
            else {
                rb.velocity = Vector2.zero;
            }
        }

        private void StopAttack() {
            attackState = AttackState.NotAttacking;
            attackStopTimer = 0;
            animator.SetTrigger("Attack_Stop");
        }

        private void PrepareAttack() {
            attackState = AttackState.Preparing;
            animator.SetTrigger("Attack");
        }


        private void Jump()
        {
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
