using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Singletons;
using UnityEngine;
using Object = System.Object;

namespace HollowKnight {
    public class Player : AbstractMikroController<HollowKnight>, ISingleton {
        public static Player Singleton {
            get {
                return SingletonProperty<Player>.Singleton;
            }
        }
        private IPlayerModel playerModel;

        [Header("Components")]
        private Rigidbody2D rb;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask groundLayer;

        [Header("Movement")]

   
        [SerializeField] private float horizontalDirection;

        private bool changingDirection => ((rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f));


        private bool canJump {
            get {
                return Input.GetButtonDown("Jump") &&
                       (onGround || this.GetModel<IPlayerModel>().RemainingExtraJump.Value > 0);
            }
        } 


        [Header("Collision Variables")] 
        [SerializeField] private bool onGround = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerModel = this.GetModel<IPlayerModel>();
            Debug.Log("Awake");

        }

        

        private void Update()
        {
            horizontalDirection = GetInput().x;
            if (canJump) Jump();
        }

        private void FixedUpdate()
        {
            CheckCollisions();
            MoveCharacter();
            if (onGround)
            {
                playerModel.ResetRemainingJumpValue();
                ApplyGroundLinearDrag();
            }
            else ApplyAirLinearDrag();
            FallMultiplier();
        }

        private Vector2 GetInput()
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void MoveCharacter()
        {
            rb.AddForce(new Vector2(horizontalDirection, 0f) * playerModel.MovementAcceleration.Value);
        }

        private void ApplyGroundLinearDrag()
        {
            if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection)
            {
                rb.drag = playerModel.GroundLinearDrag.Value;
            }
            else rb.drag = 0f;
        }

        private void ApplyAirLinearDrag()
        {
            rb.drag = playerModel.AirLinearDrag.Value;
        }

        private void Jump()
        {
            if (!onGround)
            {
                playerModel.ChangeRemainingJumpValue(-1);
            }
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * playerModel.JumpForce.Value, ForceMode2D.Impulse);
        }

        private void FallMultiplier()
        {
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = playerModel.FallMultiplier.Value;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = playerModel.LowFallMultiplier.Value;
            }
            else
            {
                rb.gravityScale = 1f;
            }

        }

        private void Dash(float x, float y)
        {

        }

        private void CheckCollisions()
        {
            
            onGround = Physics2D.Raycast(transform.position * playerModel.GroundRaycastLength.Value, Vector2.down, 
                playerModel.GroundRaycastLength.Value, groundLayer);
        }


        void ISingleton.OnSingletonInit() {
            
        }
    }

}
