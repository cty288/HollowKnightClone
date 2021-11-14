using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBPlayerMove : MonoBehaviour
{
    public Rigidbody2D rb;
    float moveX = 1.0f;
    public float moveSpeed = 5.0f;
    public float jumpPower = 5.0f;
    [SerializeField]
    bool Jump;
    [SerializeField]
    bool JumpTest;
    [SerializeField]
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void PlayerControls()
    {
        Jump = Input.GetKeyDown(KeyCode.Space);
        JumpTest = Input.GetKey(KeyCode.Space);
        moveX = Input.GetAxis("Horizontal");

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        //Debug.Log(collision.gameObject.name, collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        //Debug.Log(collision.gameObject.name, collision.gameObject);
    }

    void FixedUpdate()
    {
        PlayerControls();
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        if (isGrounded)
        {
            if (Jump)
            {
                rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                Debug.Log("Jump");
            }
        }
    }
    void Update()
    {
    }
}
