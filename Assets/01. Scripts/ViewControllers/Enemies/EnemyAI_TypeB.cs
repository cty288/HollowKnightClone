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
        [SerializeField] private float startWaitTime;

        //[SerializeField] private Animator animator;
        [SerializeField] private float patrollRangeX;
        [SerializeField] private float patrollRangeY;

        private float min_x;
        private float max_x;
        private float min_y;
        private float max_y;
        private float waitTime;

        private void Awake()
        {
            startLocation = transform.position;
            min_x = startLocation.x - patrollRangeX;
            max_x = startLocation.x + patrollRangeX;
            min_y = startLocation.y - patrollRangeY;
            max_y = startLocation.y + patrollRangeY;

            waitTime = startWaitTime;
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
                //animator.SetInteger("EnemyState", 2);
                Patrolling();
            }
            else if (currentState == EnemyState.Engaging)
            {
               
            }
            else if (currentState == EnemyState.Attacking)
            {
                //animator.SetInteger("EnemyState", 3);
            }
        }

        private void Patrolling()
        {
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(nextSpot.transform.position.x, nextSpot.transform.position.y), speed * Time.deltaTime);
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

        private Vector2 DecideDistance(Vector2 currentSpot)
        {
            Vector2 randomSpot = new Vector2(Random.Range(min_x, max_x), Random.Range(min_y, max_y));
            while (Vector2.Distance(currentSpot, randomSpot) < 5f)
            {
                randomSpot = new Vector2(Random.Range(min_x, max_x), Random.Range(min_y, max_y));
            }

            return randomSpot;
        }

        private void Update()
        {
            CheckingState(enemyState);
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
