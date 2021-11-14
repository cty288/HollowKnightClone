using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy00Movement : MonoBehaviour
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
    public float hitStun = 0.5f;
    public int side = 1;
    public Vector2 bottomOffset, rightOffset1, rightOffset2, leftOffset1, leftOffset2;
    public float collisionRadius = 0.25f;

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

        isStunned = hurtbox.GetComponent<Hurtbox>().isStunned;

        if (hurtbox.GetComponent<Hurtbox>().hitted)
        {
            //Time.timeScale = 0.5f;
            //if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Spider_Damaged"))
            //    {
            //        GetComponent<Animator>().Play("Spider_Damaged", 0, 0);
            //    Debug.Log(name);
            //}

                //StartCoroutine(Stunned(hitStun));
        }

        if (!health.dead && !isStunned)
        {
            hitbox.SetActive(true);
            distanceX = Mathf.Abs(gameObject.transform.position.x - initialX);
            if ((distanceX > maxDistanceX) && (Mathf.Sign(gameObject.transform.position.x - initialX) == Mathf.Sign(side))) //speed and direction consistant
            {
                side *= -1;
            }

            rb.velocity = new Vector3(side * walkSpeed, rb.velocity.y);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Spider_Walk"))
            {
                //Debug.Log("WHY!!!!!!!");
                animator.Play("Spider_Walk", 0, 0);
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset + new Vector2(0.75f, 0), collisionRadius);
        //Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset + new Vector2(-0.75f, 0), collisionRadius);


        RaycastHit2D detection = Physics2D.Raycast(transform.position, new Vector2(side, 0f), 20f, playerLayer);
        if (detection.collider)
        {
            //Debug.DrawRay(transform.position, new Vector2(side, 0f) * 20f, Color.red);
        }
        else
        {
            //Debug.DrawRay(transform.position, new Vector2(side, 0f) * 20f, Color.blue);
        }
    }
}
