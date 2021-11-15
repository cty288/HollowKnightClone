using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodKingMovement : MonoBehaviour
{
    public Health health;
    public Rigidbody2D rb;
    public SpriteRenderer myRenderer;

    public GameObject hitbox;
    public GameObject hitboxSlash;
    public GameObject hitboxSlam;
    public GameObject hitboxDodge;
    public GameObject hitboxFinish;
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
    public float sleepCounter = 0f;
    public float idleCounter = 0f;
    public Vector2 bottomOffset, rightOffset1, rightOffset2, leftOffset1, leftOffset2;
    public float collisionRadius = 0.25f;


    public bool isAttacking;
    public bool isSlashing;
    public bool isWandering;
    public bool isAwake;
    public bool isIdle;
    public bool isStunned;
    public bool attackCD;
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
        //onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset + new Vector2(0.75f, 0), collisionRadius, collisionLayer)
        //    || Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset + new Vector2(-0.75f, 0), collisionRadius, collisionLayer);

        isStunned = hurtbox.GetComponent<Hurtbox>().isStunned;

        RaycastHit2D detection1 = Physics2D.Raycast(transform.position, new Vector2(side, 0f), 25f, playerLayer);
        RaycastHit2D detection2 = Physics2D.Raycast(transform.position, new Vector2(-side, 0f), 25f, playerLayer);
        RaycastHit2D detection3 = Physics2D.Raycast(transform.position, new Vector2(side, 0f), 10f, playerLayer);
        RaycastHit2D detection4 = Physics2D.Raycast(transform.position, new Vector2(-side, 0f), 10f, playerLayer);

        if (!health.dead && !isStunned)
        {

            if (detection1.collider)
            {
                if (!isAwake && detection3.collider)
                {
                    isAwake = true;

                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Sleep"))
                        StartCoroutine(Wake());
                    else
                    {
                        isAttacking = true;
                        //hitbox.SetActive(true);
                    }
                }
                else if (isAwake && isWandering)
                {
                    isWandering = false;
                    isAttacking = true;
                    StopCoroutine(Wander());
                    //hitbox.SetActive(true);
                }
            }
            else if (detection2.collider)
            {

                if (!isAwake && detection4.collider)
                {
                    //side *= -1;

                    isAwake = true;

                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Sleep"))
                        StartCoroutine(Wake());
                    else
                    {
                        isAttacking = true;
                        //hitbox.SetActive(true);
                    }
                }
                else if (isAwake && isWandering)
                {
                    side *= -1;

                    isWandering = false;
                    isAttacking = true;
                    StopCoroutine(Wander());
                    //hitbox.SetActive(true);
                }
                else if (Mathf.Abs(detection2.collider.transform.position.x - transform.position.x) > 12f)
                {
                    side *= -1;
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
                    StopCoroutine(Wander());
                    StartCoroutine(Wander());
                }
                else if (isWandering)
                {
                    //sleepCounter++;
                }
            }

            if (isAttacking && !isIdle)
            {
                //hitbox.SetActive(true);

                if (detection1.collider && 
                    Mathf.Abs(detection1.collider.transform.position.x - transform.position.x) <= 4f && 
                    Mathf.Sign(detection1.collider.transform.position.x - transform.position.x) == side && 
                    !attackCD && 
                    !isSlashing)
                {
                    int rand = Random.Range(0, 4);

                    if (rand == 0)
                    {
                        StopCoroutine(Slash());
                        StartCoroutine(Slash());
                    }
                    else if (rand == 1)
                    {
                        StopCoroutine(Slam());
                        StartCoroutine(Slam());
                    }
                    else if (rand == 2)
                    {
                        StopCoroutine(Dodge());
                        StartCoroutine(Dodge());
                    }
                    else if (rand == 3)
                    {
                        StopCoroutine(Finish());
                        StartCoroutine(Finish());
                    }
                }
                else if (!isSlashing && !attackCD)
                {
                    rb.velocity = new Vector3(side * runSpeed, rb.velocity.y);

                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Walk"))
                    {
                        animator.Play("BloodKing_Walk", 0, 0);
                    }

                }

                if (!isSlashing && attackCD && isIdle)
                {

                }

            }
            else if (!isAttacking)
            {

                distanceX = Mathf.Abs(gameObject.transform.position.x - initialX);

                if ((distanceX > maxDistanceX) && (Mathf.Sign(gameObject.transform.position.x - initialX) == Mathf.Sign(side))) //speed and direction consistant
                {
                    side *= -1;
                }

                if (isWandering)
                {
                    rb.velocity = new Vector3(side * walkSpeed, rb.velocity.y);


                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Walk"))
                    {
                        animator.Play("BloodKing_Walk", 0, 0);
                    }
                }
                else if (!isAwake)  // && sleepCounter >= 120)
                {
                    sleepCounter++;

                    if (sleepCounter > 1200)
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Sleep") && !animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Wake"))
                        {
                            animator.Play("BloodKing_Sleep", 0, 0);
                        }
                    }
                    else if (sleepCounter > 1)
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Idle") && !animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Wake"))
                        {
                            animator.Play("BloodKing_Idle", 0, 0);
                        }

                    }

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
            hitboxSlash.SetActive(false);
            hitboxSlam.SetActive(false);
            hitboxDodge.SetActive(false);
            hitboxFinish.SetActive(false);
            isSlashing = false;
            attackCD = false;
            //StartCoroutine(Stunned(hitStun));
            //GetComponent<BoxCollider2D>().enabled = false;
        }

        if (side < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            hitboxSlash.transform.transform.localScale = new Vector3(-1, 1, 1); ;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
            hitboxSlash.transform.transform.localScale = new Vector3(1, 1, 1); ;
        }


        hitboxSlash.transform.transform.localScale = new Vector3(side, 1, 1); 
        hitboxSlam.transform.transform.localScale = new Vector3(side, 1, 1); 
        hitboxDodge.transform.transform.localScale = new Vector3(side, 1, 1); 
        hitboxFinish.transform.transform.localScale = new Vector3(side, 1, 1);


    }

    IEnumerator Stunned(float time)
    {
        isStunned = true;
        yield return new WaitForSeconds(time);
        isStunned = false;
    }

    IEnumerator Wander()
    {
        yield return new WaitForSeconds(1.0f);
        sleepCounter = 0;
        isWandering = true;
        yield return new WaitForSeconds(2.5f);
        isWandering = false;
        if (!isAttacking)
        {
            isAwake = false;
        }
    }

    IEnumerator Walk()
    {
        rb.velocity = new Vector3(side * runSpeed, rb.velocity.y);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Walk"))
        {
            animator.Play("BloodKing_Walk", 0, 0);
        }

        yield return new WaitForSeconds(0);
    }
    IEnumerator Idle(float time)
    {
        isIdle = true;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Idle"))
        {
            animator.Play("BloodKing_Idle", 0, 0);
        }

        yield return new WaitForSeconds(time);

        isIdle = false;
    }

    IEnumerator Slash()
    {

        isSlashing = true;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Slash"))
        {
            animator.Play("BloodKing_Slash", 0, 0);
        }

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.25f);

        hitboxSlash.SetActive(true);

        yield return new WaitForSeconds(0.75f);

        hitboxSlash.SetActive(false);
        isSlashing = false;


        attackCD = true;


                int rand = Random.Range(0, 2);

                Debug.Log(rand);

                if (rand == 0)
                {
                    StartCoroutine(Idle(1.0f));

                    yield return new WaitForSeconds(1.5f);

                    attackCD = false;
                }
                else if (rand == 1)
                {
                    side *= -1;

                    attackCD = false;

                    yield return new WaitForSeconds(1.0f);

                    StartCoroutine(Idle(1.0f));

                    yield return new WaitForSeconds(1.5f);
                }


        yield return new WaitForSeconds(1.5f);

        attackCD = false;
    }

    IEnumerator Slam()
    {

        isSlashing = true;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Slam"))
        {
            animator.Play("BloodKing_Slam", 0, 0);
        }

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.375f); //(0.375f);

        hitboxSlam.SetActive(true);

        yield return new WaitForSeconds(0.375f); //(0.625f);

        hitboxSlam.SetActive(false);
        yield return new WaitForSeconds(0.25f); //(0.625f);
        isSlashing = false;


        attackCD = true;


        int rand = Random.Range(0, 2);

        Debug.Log(rand);

        if (rand == 0)
        {
            StartCoroutine(Idle(1.0f));

            yield return new WaitForSeconds(1.5f);

            attackCD = false;
        }
        else if (rand == 1)
        {
            side *= -1;

            attackCD = false;

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(Idle(1.0f));

            yield return new WaitForSeconds(1.5f);
        }


        yield return new WaitForSeconds(1.5f);

        attackCD = false;
    }

    IEnumerator Dodge()
    {

        isSlashing = true;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Dodge"))
        {
            animator.Play("BloodKing_Dodge", 0, 0);
        }

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        //hurtbox.SetActive(false);

        yield return new WaitForSeconds(0.25f);

        hitboxDodge.SetActive(true);

        //hurtbox.SetActive(true);

        yield return new WaitForSeconds(0.75f);


        hitboxDodge.SetActive(false);

        yield return new WaitForSeconds(0.25f);

        isSlashing = false;


        attackCD = true;


        int rand = Random.Range(0, 2);

        Debug.Log(rand);

        if (rand == 0)
        {
            StartCoroutine(Idle(1.0f));

            yield return new WaitForSeconds(1.5f);

            attackCD = false;
        }
        else if (rand == 1)
        {
            side *= -1;

            attackCD = false;

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(Idle(1.0f));

            yield return new WaitForSeconds(1.5f);
        }


        yield return new WaitForSeconds(1.5f);

        attackCD = false;
    }


    IEnumerator Finish()
    {

        isSlashing = true;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Finish"))
        {
            animator.Play("BloodKing_Finish", 0, 0);
        }

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        hitboxFinish.SetActive(true);

        yield return new WaitForSeconds(0.375f);

        hitboxFinish.SetActive(false);
        yield return new WaitForSeconds(0.125f);
        isSlashing = false;


        attackCD = true;


        int rand = Random.Range(0, 2);

        Debug.Log(rand);

        if (rand == 0)
        {
            StartCoroutine(Idle(1.0f));

            yield return new WaitForSeconds(1.5f);

            attackCD = false;
        }
        else if (rand == 1)
        {
            side *= -1;

            attackCD = false;

            yield return new WaitForSeconds(1.0f);

            StartCoroutine(Idle(1.0f));

            yield return new WaitForSeconds(1.5f);
        }


        yield return new WaitForSeconds(1.5f);

        attackCD = false;
    }

    IEnumerator Wake()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Wake"))
        {
            animator.Play("BloodKing_Wake", 0, 0);
        }

        yield return new WaitForSeconds(1.0f);
        isAttacking = true;
        //hitbox.SetActive(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset + new Vector2(0.75f, 0), collisionRadius);
        //Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset + new Vector2(-0.75f, 0), collisionRadius);


        RaycastHit2D detection1 = Physics2D.Raycast(transform.position, new Vector2(side, 0f), 25f, playerLayer);
        if (detection1.collider)
        {
            Debug.DrawRay(transform.position, new Vector2(side, 0f) * 25f, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, new Vector2(side, 0f) * 25f, Color.blue);
        }
        RaycastHit2D detection2 = Physics2D.Raycast(transform.position, new Vector2(-side, 0f), 25f, playerLayer);
        if (detection2.collider)
        {
            Debug.DrawRay(transform.position, new Vector2(-side, 0f) * 25f, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, new Vector2(-side, 0f) * 25f, Color.blue);
        }
    }
}
