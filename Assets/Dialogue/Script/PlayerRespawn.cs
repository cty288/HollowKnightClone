using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerRespawn : MonoBehaviour
{
    Rigidbody2D myBody;
    SpriteRenderer myRenderer;
    Health health;
    public int batteries = 0;
    bool respawned;

    public Vector2 CheckPoint = new Vector2(-13, -5);

    void Start()
    {
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        myBody = gameObject.GetComponent<Rigidbody2D>();
        health = gameObject.GetComponent<Health>();
    }
    void Update()
    {
        if (health.dead && !respawned)
        {
            GetComponent<PlayerMovement>().audioSource.PlayOneShot(GetComponent<PlayerMovement>().audioDeath);
            GetComponent<PlayerMovement>().Animation("Player_Death");
            StartCoroutine(Respawn());
            respawned = true;
        }

    }
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1.0f);
        Color tmp = GetComponent<SpriteRenderer>().color; 
        GameObject.Find("Strawberry Manager").GetComponent<StrawberryManage>().strawberries = batteries;
        tmp.a = 0.25f;
        GetComponent<SpriteRenderer>().color = tmp;
        yield return new WaitForSeconds(0.75f);
        transform.position = CheckPoint;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        health.dead = false;
        GetComponent<PlayerMovement>().Animation("Player_Idle");
        health.invincible = true;
        StopCoroutine(GetComponent<PlayerMovement>().DisableMovement(0));
        StartCoroutine(GetComponent<PlayerMovement>().DisableMovement(0.5f));
        health.HealthPoint = health.maxHealthPoint;
        yield return new WaitForSeconds(0.75f);
        tmp.a = 1.0f;
        GetComponent<SpriteRenderer>().color = tmp;
        health.invincible = false;
        respawned = false;
        GameObject.Find("Blood King").GetComponent<Health>().HealthPoint = 20;
    }

}
