using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer myRenderer;
    public LayerMask collisionLayer;
    public GameObject hitbox;
    public GameObject hitboxCharged;
    public GameObject strawberryManager;
    public GameObject hurtbox;
    public Animator animator;
    //[SerializeField] private Sprite[] WalkSprites;
    //[SerializeField] private Sprite[] JumpSprites;
    //[SerializeField] private Sprite[] IdleSprites;

    public AudioSource audioSource;
    public AudioClip audioAttack;
    public AudioClip audioAttackCharged;
    public AudioClip audioJump;
    public AudioClip audioDash;
    public AudioClip audioWalk;
    public AudioClip audioDamage;
    public AudioClip audioLand;
    public AudioClip audioDeath;

    [Space]
    [Header("Stats")]
    public float animationSpeed = 0.3f;
    public float timer;
    public int currentSpriteIndex = 0;
    [Space]
    public float walkSpeed = 10;
    public float attackLag = 0.1f;
    public float chargeTimer = 0.0f;
    public float comboWindow = 0.1f;
    public float jumpForce = 10;
    public float dashSpeed = 20;
    public float dashLag = 0.1f;
    public float slideSpeed = 5;
    public float gravityScaler = 10;
    [Space]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    [Space]
    public Vector2 bottomOffset, rightOffset1, rightOffset2, leftOffset1, leftOffset2;
    public float collisionRadius = 0.25f;
    public float AutoClimbHeight1 = 0.5f;
    public float AutoClimbHeight2 = -0.5f;
    public float autoClimbDelay = 0.5f;

    [Space]
    [Header("Booleans")]
    public bool canMove = true;
    public bool attackWindow1;
    public bool attackWindow2;
    public bool attackWindow3;
    public bool attackWindow4;
    public bool isAttacking;
    public bool isCharging;
    public bool isChargedAttacking;
    public bool isJumping;
    public bool wallJumped;
    public bool isDashing;
    public bool hasDashed;
    public bool wallGrab;
    public bool wallSlide;
    public bool refreshed;
    public bool VariableJump;
    [Space]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public bool AutoClimableRight;
    public bool AutoClimableLeft;
    [Space]
    public int side = 1;
    public int wallSide;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        strawberryManager = GameObject.Find("Strawberry Manager");
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        collisionCheck();

        inputCheck();

        variableJump();

        timer += Time.deltaTime;

        if (side < 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;

    }
    private void collisionCheck()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, bottomOffset.y) + new Vector2(bottomOffset.x, 0), collisionRadius, collisionLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, bottomOffset.y) - new Vector2(bottomOffset.x, 0), collisionRadius, collisionLayer);

        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset1, collisionRadius, collisionLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset2, collisionRadius, collisionLayer);
        AutoClimableLeft = !Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius, collisionLayer)
            && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius, collisionLayer);
        
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset1, collisionRadius, collisionLayer)
            || Physics2D.OverlapCircle((Vector2)transform.position + rightOffset2, collisionRadius, collisionLayer);
        AutoClimableRight = !Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius, collisionLayer)
            && Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius, collisionLayer);

        onWall = onRightWall || onLeftWall;
        wallSide = onRightWall ? -1 : 1;
    }

    private void variableJump()
    {
        if (!isDashing && VariableJump)
        {
            if (rb.velocity.y < 0 && !wallGrab)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.Space))
                {
                    rb.gravityScale = gravityScaler * 0.25f;
                }
                else
                {
                    rb.gravityScale = gravityScaler * 0.75f;
                }
                if (!isAttacking && rb.velocity.y <= -1 && !onGround)
                    Animation("Player_Fall");
            }
            else if (rb.velocity.y > 0.1f && !wallSlide && !wallGrab)
            {
                if (!(Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.Space)))
                {
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                    if (!isAttacking && !onGround)
                        Animation("Player_Hop");
                }
                else if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.Space))
                {
                    if (!isAttacking)
                        Animation("Player_Jet_Jump");
                }
            }
        }
    }
    private void inputCheck()
    {
        if (onGround && !refreshed && !isDashing)
        {
            audioSource.PlayOneShot(audioLand);
            Refresh();
            refreshed = true;
        }

        if (refreshed && (!onGround || hasDashed))
        {
            refreshed = false;
        }

        if (onGround && !isDashing)
        {
            wallJumped = false;
        }

        Walk(Input.GetAxisRaw("Horizontal"));

        if (Input.GetKey(KeyCode.V) || Input.GetKey(KeyCode.J))
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer > 0.25f && !wallGrab)
            {
                isCharging = true;
                Animation("Player_Charging");
            }
        }
        if (Input.GetKeyUp(KeyCode.V) || Input.GetKeyUp(KeyCode.J))
        {
            if (chargeTimer <= 0.75f || strawberryManager.GetComponent<StrawberryManage>().strawberries <= 0)
            {
                if (attackWindow3)
                {
                    Attack3();
                }
                else if (attackWindow2)
                {
                    Attack2();
                }
                else
                {
                    Attack1();
                }


            }
            else
            {
                ChargedAttack();
            }
            chargeTimer = 0;
            isCharging = false;
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Space))
        {

            if (onGround && !isCharging)
            {
                Jump((Vector2.up), false);
            }
            else
            {
                if (onRightWall)
                {
                    StopCoroutine(DisableMovement(0));
                    StartCoroutine(DisableMovement(0.15f));
                    if (Input.GetAxisRaw("Horizontal") >= 0 && rb.velocity.y <= 0 && wallGrab)
                    {
                        //Jump((Vector2.up), true);

                        Jump((Vector2.up + Vector2.left / 2f), true);
                        side = -1;

                        //wallJumped = true;
                    }
                    else {
                        if (wallGrab || wallSlide)
                        {
                            Jump((Vector2.up  + Vector2.left / 2f), true);
                            side = -1;
                            wallJumped = true;
                        }
                        else
                        {
                            Jump((Vector2.up / 2f + Vector2.left / 2f), true);
                            side = -1;
                            wallJumped = true;
                        }
                    }
                }
                else if (onLeftWall)
                {
                    StopCoroutine(DisableMovement(0));
                    StartCoroutine(DisableMovement(0.15f));
                    if (Input.GetAxisRaw("Horizontal") <= 0 && rb.velocity.y <= 0 && wallGrab)
                    {
                        //Jump((Vector2.up), true);

                        Jump((Vector2.up + Vector2.right / 2f), true);
                        side = 1;
                        //wallJumped = true;
                    }
                    else
                    {
                        if (wallGrab || wallSlide)
                        {
                            Jump((Vector2.up  + Vector2.right / 2f), true);
                            side = 1;
                            wallJumped = true;
                        }
                        else
                        {
                            Jump((Vector2.up / 2f + Vector2.right / 2f), true);
                            side = 1;
                            wallJumped = true;
                        }
                    }
                }
            }
        }

        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKey(KeyCode.K)) && !hasDashed)
        {

            Dash(side, 0);

            /*
            if (Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Horizontal") == 0)
            {
                Dash(0, Input.GetAxisRaw("Vertical"));
            }
            else if (Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") != 0)
            {
                Dash(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }
            else if (Input.GetAxisRaw("Vertical") != 0 && Input.GetAxisRaw("Horizontal") != 0)
            {
                Dash(Input.GetAxisRaw("Horizontal") / 1.1f, Input.GetAxisRaw("Vertical") / 1.1f);
            }
            else
            {
                Dash(side * 1.5f, Input.GetAxisRaw("Vertical"));
            }
            */

        }

        if (onWall && (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.LeftShift)) && canMove)
        {
            if (!wallGrab)
            {
                wallGrab = true;
                wallJumped = false;
            }
            wallSlide = false;
            //WallGrab();
        }

        if (Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.LeftShift) || !onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
            if (!isDashing)
            {
                rb.gravityScale = gravityScaler;
            }
        }

        if (onWall && !onGround)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }
        else { 
            wallSlide = false;
        }

        if (wallGrab || wallSlide || !canMove)
            return;

        //if (Input.GetAxisRaw("Horizontal")>0)
        if (rb.velocity.x > 0.1f && Input.GetAxisRaw("Horizontal") > 0)
        {

            side = 1;
            hitbox.transform.position = transform.position + new Vector3(side * 2, 0, 0);
            if (!isChargedAttacking)
                hitboxCharged.transform.position = transform.position + new Vector3(side * 5, 1.0f, 0);
        }
        //if (Input.GetAxisRaw("Horizontal") < 0)
        if (rb.velocity.x < 0.1f && Input.GetAxisRaw("Horizontal") < 0)
        {

            side = -1;
            hitbox.transform.position = transform.position + new Vector3(side * 2, 0, 0);
            if (!isChargedAttacking)
                //hitboxCharged.transform.localScale.x = ;
                hitboxCharged.transform.position = transform.position + new Vector3(side * 5, 1.0f, 0);
        }

        }

    private void Walk(float dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (isCharging || isChargedAttacking)
            return;

        if (!wallJumped)
        {   
            if (!(onLeftWall && dir < 0) && !(onRightWall && dir > 0))
                rb.velocity = new Vector2(dir * walkSpeed, rb.velocity.y);

            if (onGround && dir != 0 && !isAttacking && !attackWindow1 && !attackWindow2 && !attackWindow3 && !attackWindow4 && !isCharging && !isDashing)
            {
                if (audioSource.clip != audioWalk && onGround)
                {
                    audioSource.clip = audioWalk;
                    audioSource.Play();
                }
                else if (!onGround)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                }
                Animation("Player_Run");
            }
            else if (onGround && dir == 0 && !isAttacking && !attackWindow1 && !attackWindow2 && !attackWindow3 && !attackWindow4 && !isCharging && !isDashing)
            {
                if (audioSource.clip == audioWalk)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                }
                Animation("Player_Idle");
            }
        }
        else
        {
            rb.velocity = new Vector2(dir * walkSpeed, rb.velocity.y);
            //rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir * walkSpeed/2f, rb.velocity.y)), 10 * Time.deltaTime);

        }

    }

    public void Attack1()
    {
        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (!canMove || isAttacking || wallGrab)
            return;

        hitbox.transform.position = transform.position + new Vector3(side * 2, 0, 0);
        Animation("Player_Attack_1");
        StartCoroutine(Attacking1());
    }
    IEnumerator Attacking1()
    {
        audioSource.PlayOneShot(audioAttack);
        isAttacking = true;
        yield return new WaitForSeconds(0.125f);
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.125f);
        hitbox.SetActive(false);
        yield return new WaitForSeconds(attackLag);
        isAttacking = false;
        attackWindow2 = true;
        yield return new WaitForSeconds(comboWindow); 
        attackWindow2 = false;
    }

    public void Attack2()
    {
        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (!canMove || isAttacking || wallGrab)
            return;

        hitbox.transform.position = transform.position + new Vector3(side * 2, 0, 0);
        //hitbox.SetActive(true);
        Animation("Player_Attack_2");
        StartCoroutine(Attacking2());
    }
    IEnumerator Attacking2()
    {
        audioSource.PlayOneShot(audioAttack);
        attackWindow2 = false;
        isAttacking = true;
        yield return new WaitForSeconds(0.125f);
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.125f);
        hitbox.SetActive(false);
        yield return new WaitForSeconds(attackLag);
        isAttacking = false;
        attackWindow3 = true;
        yield return new WaitForSeconds(comboWindow);
        attackWindow3 = false;
    }

    public void Attack3()
    {
        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (!canMove || isAttacking || wallGrab)
            return;

        hitbox.transform.position = transform.position + new Vector3(side * 2, 0, 0);
        //hitbox.SetActive(true);
        Animation("Player_Attack_3");
        StartCoroutine(Attacking3());
    }
    IEnumerator Attacking3()
    {
        audioSource.PlayOneShot(audioAttack);
        attackWindow3 = false;
        isAttacking = true;
        yield return new WaitForSeconds(0.125f);
        hitbox.SetActive(true);
        yield return new WaitForSeconds(0.125f);
        hitbox.SetActive(false);
        yield return new WaitForSeconds(attackLag);
        isAttacking = false;
        attackWindow1 = true;
        yield return new WaitForSeconds(comboWindow + 0.25f);
        attackWindow1 = false;
    }

    public void ChargedAttack()
    {
        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (!canMove || isAttacking || wallGrab)
            return;

        hitboxCharged.transform.position = transform.position + new Vector3(side * 5, 1.0f, 0);
        StartCoroutine(ChargedAttacking());
    }
    IEnumerator ChargedAttacking()
    {
        audioSource.PlayOneShot(audioAttackCharged);
        yield return new WaitForSeconds(0.1f);
        isChargedAttacking = true;
        isAttacking = true;
        hitboxCharged.SetActive(true);
        Animation("Player_Attack_Charged");
        strawberryManager.GetComponent<StrawberryManage>().strawberries -= 1;
        yield return new WaitForSeconds(0.5f);
        hitboxCharged.SetActive(false);
        yield return new WaitForSeconds(attackLag);
        isChargedAttacking = false;
        isAttacking = false;
        attackWindow4 = true;
        yield return new WaitForSeconds(comboWindow);
        attackWindow4 = false;
    }


    public void Jump(Vector2 dir, bool wall)
    {
        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        //if (!canMove)
            //return;

        rb.velocity = new Vector2(0, 0);

        if (wallJumped)
        {
            //rb.velocity = dir * jumpForce/2;
        }
        else
        {
            //rb.velocity = dir * jumpForce;
        }

        rb.velocity = dir * jumpForce;

        audioSource.PlayOneShot(audioJump);
        //StartCoroutine(Jumping());
    }
    IEnumerator Jumping()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.05f);
        isJumping = false;
    }

    private void Dash(float x, float y)
    {
        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (!canMove)
            return;

        hasDashed = true;

        //anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        //GetComponent<TrailRenderer>().enabled = true;
        Animation("Player_Dash");
        StartCoroutine(Dashing(x, y));
    }

    IEnumerator Dashing(float x, float y)
    {
        audioSource.PlayOneShot(audioDash);
        if (onGround)
        {
            //hasDashed = false;
        }
        isDashing = true;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.1f);
        //rb.velocity = new Vector2(x, y) * dashSpeed;

        RaycastHit2D detection = Physics2D.Raycast(transform.position, new Vector2(Mathf.Sign(x), 0f), dashSpeed, collisionLayer);
        if (detection.collider)
        {
            rb.transform.position += new Vector3(x, y, 0) * (detection.distance - 1);
        }
        else
        {
            rb.transform.position += new Vector3(x, y, 0) * dashSpeed;
        }

        wallJumped = true;
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        rb.velocity = new Vector2(x, y) * 12.5f;
        yield return new WaitForSeconds(dashLag);
        rb.velocity = Vector2.zero;
        rb.gravityScale = gravityScaler;
        isDashing = false;
        wallJumped = false;
        yield return new WaitForSeconds(0.0f);
        GetComponent<TrailRenderer>().enabled = false;
    }
    private void WallGrab()
    {
        rb.gravityScale = 0;

        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (onGround && !(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            rb.velocity = new Vector2(rb.velocity.x, walkSpeed * 1.0f);
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            rb.velocity = new Vector2(rb.velocity.x, walkSpeed * -1.25f);
        else if (!onGround && !isDashing && !wallJumped)
            rb.velocity = Vector2.zero;
        //else if (wallJumped)
            //rb.gravityScale *= gravityScaler;

        Animation("Player_Fall");

        //if (Input.GetAxisRaw("Horizontal") > 0)
            //side = 1;

        //if (Input.GetAxisRaw("Horizontal") < 0)
            //side = -1;

        StartCoroutine(AutoClimb());
    }

    private void WallSlide()
    {
        if (hurtbox.GetComponent<Hurtbox>().isStunned)
            return;

        if (wallSide != side)
            //anim.Flip(side * -1);

        if (!canMove)
            return;

        bool pushingWall = false;
        if ((Input.GetAxisRaw("Horizontal") > 0 && onRightWall) || (Input.GetAxisRaw("Horizontal") < 0 && onLeftWall))
        {
            if (onRightWall && !isDashing)
                side = -1;
            if (onLeftWall && !isDashing)
                side = 1;
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;
        if (rb.velocity.y <= 0)
            rb.velocity = new Vector2(push, -slideSpeed);
    }
    IEnumerator AutoClimb()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) 
        { 
            //canMove = false;
            if (onLeftWall && AutoClimableLeft)
            {
                rb.gravityScale = 0;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                yield return new WaitForSeconds(autoClimbDelay);
                transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.25f, transform.position.z);
                rb.gravityScale = gravityScaler;
            }
            if (onRightWall && AutoClimableRight)
            {
                rb.gravityScale = 0;
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                yield return new WaitForSeconds(autoClimbDelay);
                transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.25f, transform.position.z);
                rb.gravityScale = gravityScaler;
            }
            //canMove = true;
        }
    }
    public IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void Refresh()
    {
        isDashing = false;
        hasDashed = false;
        isJumping = false;
        wallJumped = false;
        VariableJump = true;
        //attackWindow1 = true;
    }

    public void Animation(string anim)
    {
        if (GetComponent<Health>().dead && anim != "Player_Death")
            return;

        if (hurtbox.GetComponent<Hurtbox>().isStunned && anim != "Player_Damaged")
            return;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(anim))
        {
            animator.Play(anim, 0, 0);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0, bottomOffset.y) + new Vector2(bottomOffset.x, 0), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(0, bottomOffset.y) - new Vector2(bottomOffset.x, 0), collisionRadius);

        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset1, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset1, collisionRadius);

        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset2, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset2, collisionRadius);

        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight1), collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset1.x, 0) + new Vector2(0, AutoClimbHeight2), collisionRadius); 
        
        RaycastHit2D detection = Physics2D.Raycast(transform.position, new Vector2(side, 0f), dashSpeed, collisionLayer);
        if (detection.collider)
        {
            Debug.DrawRay(transform.position, new Vector2(side, 0f) * dashSpeed, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, new Vector2(side, 0f) * dashSpeed, Color.blue);
        }
    }

    /*private void PlayerAnimation(Sprite[] currentSprite)
    {
        timer += Time.deltaTime;
        if (timer >= animationSpeed)
        {
            timer = 0;
            currentSpriteIndex++;
            currentSpriteIndex %= currentSprite.Length;
        }
        myRenderer.sprite = currentSprite[currentSpriteIndex];
    }
    */
}
