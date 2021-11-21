using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class simpleRangeAI : MonoBehaviour
    {

        public float followSpeed;
        public float wanderSpeed;
        public float lineOfSite;
        public float shootingRange;
        public float fireRate = 3;
        private float nextFireTime;

        public bool Wander;

        public GameObject bullet;
        public GameObject bulletParent;
        private Transform player;
        public Rigidbody2D rb;

        public float latestDirectionChangeTime;
        public readonly float directionChangeTime = 3f;
        public Vector2 movementDirection;
        private Vector2 movementPerSecond;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Player").transform;
            latestDirectionChangeTime = 0f;
            calcuateNewMovementVector();
            Wander = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, lineOfSite);
            Gizmos.DrawWireSphere(transform.position, shootingRange);
        }

        void calcuateNewMovementVector()
        {
            //create a random direction vector with the magnitude of 1, later multiply it with the velocity of the enemy
            movementDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            movementPerSecond = movementDirection * wanderSpeed;
        }


        IEnumerator WaitForSeconds()
        {
            yield return new WaitForSeconds(1.5f);
            Wander = true;
            latestDirectionChangeTime = Time.time;
            calcuateNewMovementVector();
        }

        // Update is called once per frame
        void Update()
        {
            float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
            if (distanceFromPlayer < lineOfSite && distanceFromPlayer > shootingRange)
            {
                rb.isKinematic = false;
                transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(player.position.x, player.position.y + 3), followSpeed * Time.deltaTime);
            }
            else if (distanceFromPlayer <= shootingRange && nextFireTime < Time.time)
            {
                rb.isKinematic = true;
                Instantiate(bullet, bulletParent.transform.position, Quaternion.identity);
                nextFireTime = Time.time + fireRate;
            }

            //if the changeTime was reached, calculate a new movement vector
            if (Time.time - latestDirectionChangeTime > directionChangeTime)
            {
                latestDirectionChangeTime = Time.time;
                StartCoroutine(WaitForSeconds());
                Wander = false;
            }

            //move enemy: 
            if (distanceFromPlayer > lineOfSite && Wander == true)
            {
                transform.position = new Vector2(transform.position.x + (movementPerSecond.x * Time.deltaTime),
                transform.position.y + (movementPerSecond.y * Time.deltaTime));
            }
        }
    }
}
