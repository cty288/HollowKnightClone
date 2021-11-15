using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

namespace HollowKnight
{
    public class Movement : MonoBehaviour
    {
        [Header("Components")]
        private Rigidbody2D rb;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask groundLayer;

        [Header("Movement")]
        [SerializeField] private float movementAcceleration;
        [SerializeField] private float maxMoveSpeed;
        [SerializeField] private float groundLinearDrag;
        [SerializeField] private float horizontalDirection;

        private bool changingDirection => ((rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f));

        [Header("Jump")] 
        [SerializeField] private float jumpForce = 12f;
        [SerializeField] private float airLinearDrag = 2.5f;
        [SerializeField] private float fallMultiplier = 8f;
        [SerializeField] private float lowFallMultiplier = 5f;
        [SerializeField] private int extraJumps = 2;
        private int extraJumpsValue;
        private bool canJump => Input.GetButtonDown("Jump") && (onGround || extraJumpsValue > 0);

        [Header("Dash")]
        [SerializeField] private float dashSpeed;
        [SerializeField] private bool isDashing;
        [SerializeField] private bool hasDash;

        [Header("Collision Variables")]
        [SerializeField] private float groundRaycastLength;
        [SerializeField] private bool onGround = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
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
                extraJumpsValue = extraJumps;
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
            rb.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);
        }

        private void ApplyGroundLinearDrag()
        {
            if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection)
            {
                rb.drag = groundLinearDrag;
            }
            else rb.drag = 0f;
        }

        private void ApplyAirLinearDrag()
        {
            rb.drag = airLinearDrag;
        }

        private void Jump()
        {
            if (!onGround)
            {
                extraJumpsValue--;
            }
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void FallMultiplier()
        {
            if(rb.velocity.y < 0)
            {
                rb.gravityScale = fallMultiplier;
            }
            else if(rb.velocity.y > 0 && ! Input.GetButton("Jump"))
            {
                rb.gravityScale = lowFallMultiplier;
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
            onGround = Physics2D.Raycast(transform.position * groundRaycastLength, Vector2.down, groundRaycastLength, groundLayer);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundRaycastLength);
        }


    }
}