using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 1.0f;

    public GameObject owner;

    private void Start()
    {
        owner = transform.parent.gameObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Hurtbox") || !collision.gameObject.GetComponent<Hurtbox>())
        {
            return;
        }

        Hurtbox victim = collision.gameObject.GetComponent<Hurtbox>();
        if (victim.hitted == false && !victim.owner.GetComponent<Health>().invincible && victim.owner.GetComponent<Health>().HealthPoint > 0)
        {
            if (name == "Fall Collider")
            {
                victim.dir = new Vector2(0, victim.transform.position.y - transform.position.y);
            }
            else
            {
                victim.dir = victim.transform.position - owner.transform.position;
            }
            victim.hitbox = gameObject;
            victim.hitted = true;
            victim.owner.GetComponent<Health>().HealthPoint -= damage;
        }
    }
}
