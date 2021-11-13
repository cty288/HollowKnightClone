using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class movement : MonoBehaviour
    {
        public float walkSpeed = 3.0f;

        public Rigidbody2D rb;

        Vector2 playerMovement;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            playerMovement.x = Input.GetAxisRaw("Horizontal");
            playerMovement.y = Input.GetAxisRaw("Vertical");
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + playerMovement * walkSpeed * Time.fixedDeltaTime);
        }
    }
}
