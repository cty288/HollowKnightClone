using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class EnemyAI_TypeB : MonoBehaviour
    {
        [SerializeField] private EnemyState enemyState;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 startLocation;
        [SerializeField] private Transform nextSpot;
        [SerializeField] private GameObject target;
        [SerializeField] private float startWaitTime;
        [SerializeField] private float attackRate;

        [SerializeField] private Animator animator;
        [SerializeField] private float patrollRangeX;
        [SerializeField] private float patrollRangeY;
        [SerializeField] private Vector3 spriteScale;

        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private bool canFire = true;
        [SerializeField] private bool facingRight = true;

        private float min_x;
        private float max_x;
        private float min_y;
        private float max_y;
        private float waitTime;
        private float attackTimer;
        private float p;
        float startLocation_y;
        Vector2 nextAttackSpotCenter;

        private void Awake()
        {
            startLocation = transform.position;
            min_x = startLocation.x - patrollRangeX;
            max_x = startLocation.x + patrollRangeX;
            min_y = startLocation.y - patrollRangeY;
            max_y = startLocation.y + patrollRangeY;

            waitTime = startWaitTime;
            spriteScale = transform.localScale;
            nextAttackSpotCenter = transform.position;
            startLocation_y = transform.position.y;
        }
        public enum EnemyState
        {
            Patrolling,
            Engaging,
            Attacking,
            dodging,
        }

        private void CheckingState(EnemyState currentState)
        {
            if (currentState == EnemyState.Patrolling)
            {
                animator.SetInteger("EnemyState", 1);
                Patrolling();
            }
            else if (currentState == EnemyState.Engaging)
            {
                if (canFire) Fire();
                else StartCoroutine(Dodging());
            }
            else if (currentState == EnemyState.Attacking)
            {
                //Dodging();
            }
        }


        private void Patrolling()
        {
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(nextSpot.transform.position.x, nextSpot.transform.position.y), speed * Time.deltaTime);
            if ((nextSpot.transform.position.x - transform.position.x) < 0) spriteScale.x = 1;
            else if ((nextSpot.transform.position.x - transform.position.x) > 0) spriteScale.x = -1;
            if (Vector2.Distance(transform.position, nextSpot.position) < 0.25f)
            {   
                if (waitTime <= 0)
                {
                    nextSpot.position = DecideDistance(transform.position);
                    waitTime = startWaitTime;
                }
                else
                {
                    //animator.SetInteger("EnemyState", 1);
                    waitTime -= Time.deltaTime;
                }
            }
        }

        private void Fire()
        {
            if (attackTimer <= 0)
            {
                animator.SetInteger("EnemyState", 2);
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
               
                
                attackTimer = attackRate;
                canFire = false;
            }
            else
            {
                attackTimer -= Time.deltaTime;
            }
        }

        private bool CheckReached(Vector2 nextSpot)
        {
            if (Vector2.Distance(transform.position, nextSpot) <= 0.5)
                return true;
            return false;
        }
        private bool CheckPlayerDistance()
        {
            if (Vector2.Distance(transform.position, target.transform.position) <= 5)
                return true;
            return false;
        }

        private IEnumerator Dodging()
        {
            float moveRange = 2f;

            if ((nextAttackSpotCenter.x - transform.position.x) <= 0) spriteScale.x = 1;
            else if ((nextAttackSpotCenter.x - transform.position.x) > 0) spriteScale.x = -1;

            if (CheckPlayerDistance())
            {
                transform.position += new Vector3 (nextAttackSpotCenter.x, nextAttackSpotCenter.y) * 1.5f * Time.deltaTime;
            }

            else if (!CheckReached(nextAttackSpotCenter))
            {
                transform.position = Vector2.MoveTowards(transform.position,
                  nextAttackSpotCenter, speed * Time.deltaTime);
            }

            //if (CheckReached(nextAttackSpotCenter))
            
            p = Random.Range(0f, 100f);

            yield return new WaitForSeconds(.7f);

            nextAttackSpotCenter = new Vector2(Random.Range(-2f, 2f), Random.Range(startLocation_y - 1, startLocation_y + 1));
            canFire = true;
        }


        private IEnumerator CoroutineAttack()
        {
            animator.SetInteger("EnemyState", 2);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(0.5f);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            animator.SetInteger("EnemyState", 1);
        }

        private Vector2 DecideDistance(Vector2 currentSpot)
        {
            Vector2 randomSpot = new Vector2(Random.Range(min_x, max_x), Random.Range(min_y, max_y));
            while (Vector2.Distance(currentSpot, randomSpot) < 5f)
            {
                randomSpot = new Vector2(Random.Range(min_x, max_x), Random.Range(min_y, max_y));
            }

            return randomSpot;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                target = collision.gameObject;
                enemyState = EnemyState.Engaging;
            }
        }

        private void Update()
        {
            /*
            if (transform.position.x - target.transform.position.x < 0 && !facingRight)
            {
                Flip();
            }
            
            else if (transform.position.x - target.transform.position.x >= 0 && facingRight)
            { 
                Flip();
            }
            if (transform.position.x - target.transform.position.x >= 0) transform.Rotate(0f, 180f, 0f);
            */
            CheckingState(enemyState);
            CheckPlayer();
            transform.localScale = spriteScale;
        }     
        
        void Flip()
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        void CheckPlayer()
        {
            if (target != null)
            {
                if (Vector2.Distance(target.transform.position, this.transform.position) >= patrollRangeX)
                {
                    enemyState = EnemyState.Patrolling;
                }
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(min_x, max_y), new Vector2(max_x, max_y));
            Gizmos.DrawLine(new Vector2(min_x, min_y), new Vector2(max_x, min_y));
            Gizmos.DrawLine(new Vector2(min_x, max_y), new Vector2(min_x, min_y));
            Gizmos.DrawLine(new Vector2(max_x, max_y), new Vector2(max_x, min_y));
        }
    }
}
