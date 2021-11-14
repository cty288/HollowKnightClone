using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Vector3 RespawnOffset = new Vector3(0, 0, 0);
    public float batteries = 0;
    public bool Disable = false;
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("Ahhhahh!");
            collision.GetComponent<PlayerRespawn>().CheckPoint = transform.position + RespawnOffset;
            collision.GetComponent<PlayerRespawn>().batteries = GameObject.Find("Strawberry Manager").GetComponent<StrawberryManage>().strawberries;

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("CheckPoint_Start"))
            {
                animator.Play("CheckPoint_Start", 0, 0);
            }

            if (Disable)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().Animation("Player_Idle");
                StopCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().DisableMovement(0));
                StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().DisableMovement(1.0f));
            }

         
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerRespawn>().CheckPoint = transform.position + RespawnOffset;

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("CheckPoint_Down"))
            {
                animator.Play("CheckPoint_Down", 0, 0);
            }
        }
    }
}
