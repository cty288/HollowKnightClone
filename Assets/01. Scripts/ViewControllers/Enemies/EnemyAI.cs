using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] private EnemyState enemyState;

        [SerializeField] private float speed;
        [SerializeField] private Vector2 startLocation;
        [SerializeField] private float startWaitTime;

        [SerializeField] private Transform nextSpot;
        
        [SerializeField] private float attackRange;
        [SerializeField] private GameObject target;

        [SerializeField] private float patrolRange;
        [SerializeField] private SpriteRenderer spriteRender;

        [SerializeField] private Animator animator;
        [SerializeField] private Vector3 spriteScale;

        private float min_X;
        private float max_X;
        private float waitTime;

        public enum EnemyState
        {
            Patrolling,
            Chasing,
            Attacking
        }

        private void Awake()
        {
            startLocation = transform.position;
            min_X = startLocation.x - patrolRange;
            max_X = startLocation.x + patrolRange;
            waitTime = startWaitTime;
            spriteScale = transform.localScale;

            nextSpot.position = new Vector2(Random.Range(min_X, max_X), startLocation.y);
        }

        private void CheckingState(EnemyState currentState)
        {
            if (currentState == EnemyState.Patrolling)
            {
                animator.SetInteger("EnemyState", 2);
                Patrolling();
            }
            else if(currentState == EnemyState.Chasing)
            {
                Chasing();
            }
            else if(currentState == EnemyState.Attacking)
            {
                animator.SetInteger("EnemyState", 3);
            }
        }
        private EnemyState CheckingAttack()
        {
            if (attackRange >= Vector2.Distance(target.transform.position, this.transform.position))
            {
                return EnemyState.Attacking;
            }
            else return EnemyState.Chasing;
        }

        private void Patrolling()
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextSpot.transform.position.x, startLocation.y), speed * Time.deltaTime);
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
                    animator.SetInteger("EnemyState", 1);
                    waitTime -= Time.deltaTime; 
                }
            }
        }

        private Vector2 DecideDistance(Vector2 currentSpot)
        {
            Vector2 randomSpot = new Vector2(Random.Range(min_X, max_X), startLocation.y);
            while (Vector2.Distance(currentSpot, randomSpot) < 5f)
            {
                randomSpot = new Vector2(Random.Range(min_X, max_X), startLocation.y);
            }
           
            return randomSpot;
        }

        private void Chasing()
        {
            animator.SetInteger("EnemyState", 3);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.transform.position.x, startLocation.y), speed * Time.deltaTime);
            if ((target.transform.position.x - transform.position.x) <= 0) spriteScale.x = 1;
            else spriteScale.x = -1;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                target = collision.gameObject;
                enemyState = CheckingAttack();
            }
        }

        private void Update()
        {
            CheckingState(enemyState);
            if (target != null)
            {
                if (Vector2.Distance(target.transform.position, this.transform.position) >= patrolRange)
                {
                    enemyState = EnemyState.Patrolling;
                }
            }
            transform.localScale = spriteScale;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(min_X, startLocation.y), new Vector2(max_X, startLocation.y));
        }
    }
}
