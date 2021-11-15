using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class bullet : MonoBehaviour
    {
        GameObject target;
        public float bulletSpeed;
        Rigidbody2D rb;

        private CircleCollider2D cc;

        private void Awake()
        {
            cc = GetComponent<CircleCollider2D>();
            cc.isTrigger = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            target = GameObject.Find("Player");
            Vector2 moveDir = (target.transform.position - transform.position).normalized * bulletSpeed;
            rb.velocity = new Vector2(moveDir.x, moveDir.y);
            Destroy(this.gameObject, 2);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {

                Destroy(this.gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
