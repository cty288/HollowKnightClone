using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
	public GameObject owner;
    public GameObject hitbox;
    public GameObject strawberryManager;
    public Health health;
    //public Animator animator;
    //public Animation animationDamaged;

    public bool hitted;
    public bool isStunned;
    public float hitCD = 0.5f;
    public float hitStun = 0.5f;
    float stunCounter = 0;
    float invincibleCounter = 0;
    public Vector2 dir;

    public Rigidbody2D Rigidbody2D;

    private void Start()
    {
        owner = transform.parent.gameObject;
        strawberryManager = GameObject.Find("Strawberry Manager");
        health = owner.GetComponent<Health>();
        //animator = owner.GetComponent<Animator>();
    }

    private void Update()
    {
        if (hitted)
        {
            AudioSource.PlayClipAtPoint(GameObject.Find("Madeline").GetComponent<PlayerMovement>().audioDamage, transform.position, 1);

            StartCoroutine(Stunned(hitStun));
            StartCoroutine(hitRecovery());

            if (owner.name == "Madeline")
            {
                owner.GetComponent<PlayerMovement>().StartCoroutine(owner.GetComponent<PlayerMovement>().DisableMovement(0.25f));
                owner.GetComponent<PlayerMovement>().side = (int)Mathf.Sign(-dir.x);

            }

            if (owner.name == "Blood King") 
            {
                if (owner.GetComponent<BloodKingMovement>().isSlashing)
                {
                    owner.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    //owner.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(dir.x) * 10.0f, Mathf.Sign(dir.y + 1) * 10.0f);
                    return;
                }
            }

            /*
            owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Slash") ||
            owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Slam") ||
            owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Dodge") ||
            owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Finish"))
            */

            if (hitbox.name == "Fall Collider")
                return;

            owner.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            owner.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(dir.x) * 20.0f, Mathf.Sign(dir.y+1) * 20.0f);

        }

        if (isStunned)
        {
            stunCounter++;
            if (stunCounter >= hitStun * 300)
                isStunned = false;
        }
        else
        {
            stunCounter = 0;
        }

        if (health.invincible)
        {
            invincibleCounter++;
            if (invincibleCounter >= hitCD * 300)
                health.invincible = false;
        }
        else
        {
            invincibleCounter = 0;
        }

    }

    IEnumerator Stunned(float time)
    {
        //if (!isStunned)
        if (owner.name == "Blood King")
        {
            if (!owner.GetComponent<BloodKingMovement>().isSlashing)
            {
                isStunned = true;
            }
        }
        else
        {
            isStunned = true;
        }
        yield return new WaitForSeconds(time);
        isStunned = false;
    }

    IEnumerator hitRecovery()
    {
        hitted = false;

        //if (!health.invincible)
        health.invincible = true;

        Color tmp = owner.GetComponent<SpriteRenderer>().color;

        if (owner.name == "Madeline")
        {
            Time.timeScale = 0.1f;
            owner.GetComponent<PlayerMovement>().Animation("Player_Damaged");
        }

        if (owner.name == "Spider")
        {
            Time.timeScale = 0.5f;
            if (!owner.GetComponent <Animator>().GetCurrentAnimatorStateInfo(0).IsName("Spider_Damaged"))
            {
                owner.GetComponent<Animator>().Play("Spider_Damaged", 0, 0);
            }
        }

        if (owner.name == "Flower")
        {
            Time.timeScale = 0.5f;
            if (!owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Flower_Damaged"))
            {
                owner.GetComponent<Animator>().Play("Flower_Damaged", 0, 0);
            }
        }

        if (owner.name == "Blood King")
        {
            Time.timeScale = 0.5f;
            if (!owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Damaged"))
            {
                
                if (//!owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Slash") && 
                    !owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Slam") && 
                    !owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Dodge") && 
                    !owner.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Finish"))
                {
                
                    owner.GetComponent<Animator>().Play("BloodKing_Damaged", 0, 0);
                }
                else
                {
                    owner.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            owner.GetComponent<BloodKingMovement>().hitbox.SetActive(false);
            owner.GetComponent<BloodKingMovement>().hitboxSlash.SetActive(false);
            owner.GetComponent<BloodKingMovement>().hitboxSlam.SetActive(false);
            owner.GetComponent<BloodKingMovement>().hitboxDodge.SetActive(false);
            owner.GetComponent<BloodKingMovement>().hitboxFinish.SetActive(false);
        }

        yield return new WaitForSeconds(0.05f);

        Time.timeScale = 1.0f;

        yield return new WaitForSeconds(hitCD);

        if (!owner.GetComponent<Health>().dead)
        {
            tmp.a = 1.0f;
            owner.GetComponent<SpriteRenderer>().color = tmp;
        }

        health.invincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (owner.name == "Madeline" && collision.gameObject.tag == "Strawberry")
        {
            Destroy(collision.gameObject);
            if (strawberryManager.GetComponent<StrawberryManage>().strawberries <5)
                strawberryManager.GetComponent<StrawberryManage>().strawberries++;
        }

        if (collision.gameObject.name == "Fall Collider")
        {
            health.dead = true;
        }

        if (collision.gameObject.name == "Jump Pad")
        {
            owner.GetComponent<PlayerMovement>().VariableJump = false;
            owner.GetComponent<Rigidbody2D>().gravityScale = owner.GetComponent<PlayerMovement>().gravityScaler;
            owner.GetComponent<PlayerMovement>().Jump((Vector2.up), true);
        }

    }
}
