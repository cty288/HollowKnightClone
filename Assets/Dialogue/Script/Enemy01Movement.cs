using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01Movement : MonoBehaviour
{
    public Health health;
    public Rigidbody2D rb;
    public SpriteRenderer myRenderer;
    public GameObject hitbox;
    public GameObject hurtbox;
    public LayerMask collisionLayer;
    public LayerMask playerLayer;
    public Animator animator;

    public float initialX;
    public float distanceX;
    public float maxDistanceX;
    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    public float hitStun = 0.5f;
    public int side = 1;
    public Vector2 bottomOffset, rightOffset1, rightOffset2, leftOffset1, leftOffset2;
    public float collisionRadius = 0.25f;


    public bool isAttacking;
    public bool isWandering;
    public bool isAwake;
    public bool isStunned;
    public bool onGround;

    void Start()
    {
        health = GetComponent<Health>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        initialX = gameObject.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset + new Vector2(0.75f, 0), collisionRadius, collisionLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset + new Vector2(-0.75f, 0), collisionRadius, collisionLayer);

        isStunned = hurtbox.GetComponent<Hurtbox>().isStunned;

        if (!health.dead && !isStunned)
        {
            RaycastHit2D detection1 = Physics2D.Raycast(transform.position, new Vector2(side, 0f), 5f, playerLayer);
            RaycastHit2D detection2 = Physics2D.Raycast(transform.position, new Vector2(-side, 0f), 5f, playerLayer);

            if (detection1.collider && !isStunned)
            {
                if (!isAwake)
                {
                    isAwake = true;

                    StartCoroutine(Wake());
                }
                else if (isWandering)
                {
                    isWandering = false;
                    isAttacking = true;
                    hitbox.SetActive(true);
                }
            }
            else if (detection2.collider && !isStunned)
            {
                if (!isAwake)
                {
                    side *= -1;

                    isAwake = true;

                    StartCoroutine(Wake());
                }
                else if (isWandering)
                {
                    side *= -1;
                    isWandering = false;
                    isAttacking = true;
                    hitbox.SetActive(true);
                }
            }
            else
            {
                if (isAttacking)
                {
                    isAttacking = false;
                    hitbox.SetActive(false);
                }
                else if (isAwake && !isWandering)
                {
                    StartCoroutine(Wander());
                }
            } 

            distanceX = Mathf.Abs(gameObject.transform.position.x - initialX);

            if (!isAttacking)
            {
                if ((distanceX > maxDistanceX) && (Mathf.Sign(gameObject.transform.position.x - initialX) == Mathf.Sign(side))) //speed and direction consistant
                {
                    side *= -1;
                }

                if (isWandering)
                {
                    rb.velocity = new Vector3(side * walkSpeed, rb.velocity.y);


                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Flower_Walk"))
                    {
                        animator.Play("Flower_Walk", 0, 0);
                    }
                }
                else if (!isAwake)
                {

                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Flower_Idle") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Flower_Wake"))
                    {
                        animator.Play("Flower_Idle", 0, 0);
                    }

                }

            }
            else
            {
                hitbox.SetActive(true);
                //side = Mathf.Sign(gameObject.transform.position.x - GameObject.FindGameObjectsWithTag("Player").transform.position.x);
                rb.velocity = new Vector3(side * runSpeed, rb.velocity.y);

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Flower_Walk"))
                {
                    animator.Play("Flower_Walk", 0, 0);
                }
            }

            if (hurtbox.GetComponent<Hurtbox>().hitted)
            {
                //StartCoroutine(Stunned(hitStun));
                //Debug.Log("Stunned!!!");
            }

        }
        else
        {
            hitbox.SetActive(false);
            //GetComponent<BoxCollider2D>().enabled = false;
        }

        if (side < 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;




    }

    IEnumerator Stunned(float time)
    {
        isStunned = true;
        yield return new WaitForSeconds(time);
        isStunned = false;
    }

    IEnumerator Wander()
    {
        isWandering = true;
        yield return new WaitForSeconds(1.5f);
        isWandering = false;
        if (!isAttacking)
        {
            isAwake = false;
        }
    }
    IEnumerator Wake()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Flower_Wake"))
        {
            animator.Play("Flower_Wake", 0, 0);
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = true;
        hitbox.SetActive(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset + new Vector2(0.75f, 0), collisionRadius);
        //Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset + new Vector2(-0.75f, 0), collisionRadius);


        RaycastHit2D detection1 = Physics2D.Raycast(transform.position, new Vector2(side, 0f), 5f, playerLayer);
        if (detection1.collider)
        {
            Debug.DrawRay(transform.position, new Vector2(side, 0f) * 5f, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, new Vector2(side, 0f) * 5f, Color.blue);
        }
        RaycastHit2D detection2 = Physics2D.Raycast(transform.position, new Vector2(-side, 0f), 5f, playerLayer);
        if (detection2.collider)
        {
            Debug.DrawRay(transform.position, new Vector2(-side, 0f) * 5f, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, new Vector2(-side, 0f) * 5f, Color.blue);
        }
    }
}
