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

        private float min_x;
        private float max_x;
        private float min_y;
        private float max_y;
        private float waitTime;
        private float attackTimer;
        private float p;
        bool reached = true;
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
            nextAttackSpotCenter = new Vector2 (-10,10);
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
                Vector3 shootDir = (target.transform.position - firePoint.transform.position).normalized;
                bullet.GetComponent<Bullet>().SetDir(shootDir);
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
            if (Vector2.Distance(transform.position, nextSpot) == 0)
                return true;
            return false;
        }

        private IEnumerator Dodging()
        {
            float attackRange = 10f;

            if (CheckReached(nextAttackSpotCenter))
            {
                p = Random.Range(0f, 100f);

                if (p <= 50)
                {
                    //nextAttackSpotCenter = new Vector2(target.transform.position.x + attackRange, transform.position.y);
                    nextAttackSpotCenter = new Vector2(50, 10);
                }
                else
                {
                    //nextAttackSpotCenter = new Vector2(target.transform.position.x - attackRange, transform.position.y);
                    nextAttackSpotCenter = new Vector2(-50, 10);
                }
            }
            if (!CheckReached(nextAttackSpotCenter))
            {
                transform.position = Vector2.MoveTowards(transform.position,
                  new Vector2(Random.Range(nextAttackSpotCenter.x - 3, nextAttackSpotCenter.x + 3),
                  Random.Range(nextAttackSpotCenter.y - 3, nextAttackSpotCenter.y + 3)), speed * Time.deltaTime);
                yield return new WaitForSeconds(0.8f);
            }
           
            canFire = true;
            
            Debug.Log(nextAttackSpotCenter.x);
            Debug.Log(reached);
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
            CheckingState(enemyState);
            CheckPlayer();
            transform.localScale = spriteScale;
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
