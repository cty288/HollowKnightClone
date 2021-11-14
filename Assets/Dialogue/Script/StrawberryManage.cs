using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StrawberryManage : MonoBehaviour
{
    public int strawberries = 0;

    public TMP_Text text;
    public TMP_Text health;

    void Start()
    {
        
    }

    void Update()
    {
        displayNum();

        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        Vector3 Door1Pos = GameObject.Find("Door 5/5").transform.position;
        Vector3 Door2Pos = GameObject.Find("Door Boss").transform.position;
        Vector3 Door3Pos = GameObject.Find("Door End").transform.position;

        if (Mathf.Abs(playerPos.x - Door1Pos.x) < 7.5f && (playerPos.y - Door1Pos.y) < 5 && (playerPos.y - Door1Pos.y) > 0)
        {
            if (!GameObject.Find("Door 5/5").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Door_Open"))
                GameObject.Find("Door 5/5").GetComponent<Animator>().Play("Door_Open", 0, 0);

            //GameObject.Find("Door 5/5").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Door 5/5").GetComponent<BoxCollider2D>().enabled = false;
        }

        if (Mathf.Abs(playerPos.y - Door2Pos.y) < 3 && (playerPos.x - Door2Pos.x) > 10f && !GameObject.Find("Blood King").GetComponent<Health>().dead)
        {
            if (!GameObject.Find("Door Boss").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Door_Close"))
                GameObject.Find("Door Boss").GetComponent<Animator>().Play("Door_Close", 0, 0);

            //GameObject.Find("Door 5/5").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Door Boss").GetComponent<BoxCollider2D>().enabled = true;
        }
        else if(GameObject.Find("Blood King").GetComponent<Health>().dead || (Mathf.Abs(playerPos.y - Door2Pos.y) < 10 && (playerPos.x - Door2Pos.x) > -10f && (playerPos.x - Door2Pos.x) < 0))
        {
            if (!GameObject.Find("Door Boss").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Door_Open"))
                GameObject.Find("Door Boss").GetComponent<Animator>().Play("Door_Open", 0, 0);

            //GameObject.Find("Door 5/5").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Door Boss").GetComponent<BoxCollider2D>().enabled = false;

        }
        else
        {
            if (!GameObject.Find("Door Boss").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Door_Close"))
                GameObject.Find("Door Boss").GetComponent<Animator>().Play("Door_Close", 0, 0);

            //GameObject.Find("Door 5/5").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Door Boss").GetComponent<BoxCollider2D>().enabled = true;

        }

        if (GameObject.Find("Blood King").GetComponent<Health>().dead && (Mathf.Abs(playerPos.y - Door3Pos.y) < 10 && Mathf.Abs(playerPos.x - Door3Pos.x) < 10f))
        {
            if (!GameObject.Find("Door End").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Door_Open"))
                GameObject.Find("Door End").GetComponent<Animator>().Play("Door_Open", 0, 0);

            GameObject.Find("Door End").GetComponent<BoxCollider2D>().enabled = false;

        }
        else
        {
            if (!GameObject.Find("Door End").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Door_Close"))
                GameObject.Find("Door End").GetComponent<Animator>().Play("Door_Close", 0, 0);

            GameObject.Find("Door End").GetComponent<BoxCollider2D>().enabled = true;

        }

        if (strawberries >= 5)
        {
            GameObject.Find("Theo 0/5").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Theo 0/5").GetComponent<DialogueTrigger>().enabled = false;
            GameObject.Find("Theo 5/5").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Theo 5/5").GetComponent<DialogueTrigger>().enabled = true;
        }
    }
    void displayNum()
    {
        text.text = ("  " + strawberries + " / 5 Batteries");
        health.text = ("  " + GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().HealthPoint + " / 5 HP");
    }
}
