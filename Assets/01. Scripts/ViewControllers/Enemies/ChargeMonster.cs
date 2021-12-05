using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HollowKnight
{
    public class ChargeMonster : AbstractAbsorbableCanAttackEnemy<ChargeMonsterConfigurtion, ChargeMonsterConfigurtion.ChargeMonsterStages> {
        [SerializeField] private float chaseSpeed;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 startLocation;
        [SerializeField] private float startWaitTime;
        [SerializeField] private float dizzyTime = 3f;

        [SerializeField] private Vector2 nextSpot;

        [SerializeField] private SpriteRenderer aliveSpriteRenderer;
        [SerializeField] private SpriteRenderer deathSpriteRenderer;
     
        [SerializeField] private GameObject target;

        [SerializeField] private float patrolRange;
        [SerializeField] private Trigger2DCheck attackTrigger2DCheck;

        [SerializeField] private Animator animator;
       

        private float min_X;
        private float max_X;
        [SerializeField]
        private float waitTime;

      

        private void Awake()
        {
            base.Awake();
            startLocation = transform.position;
            min_X = startLocation.x - patrolRange;
            max_X = startLocation.x + patrolRange;
            waitTime = startWaitTime;
          

            nextSpot = new Vector2(Random.Range(min_X, max_X), startLocation.y);
        }

        protected override void OnFSMStage(ChargeMonsterConfigurtion.ChargeMonsterStages currentStage)
        {
            //Debug.Log(currentStage.ToString());
            if (currentStage == ChargeMonsterConfigurtion.ChargeMonsterStages.Patrolling)
            {
                //Debug.Log("Patrolling");
                animator.SetInteger("EnemyState", 2);
                Patrolling();
            }
            else if (currentStage == ChargeMonsterConfigurtion.ChargeMonsterStages.Chasing) {
                animator.SetInteger("EnemyState", 3);
                Chasing();
            }
            else if (currentStage == ChargeMonsterConfigurtion.ChargeMonsterStages.Attacking)
            {
                //attacking
                animator.SetInteger("EnemyState", 3);
            }else if (currentStage == ChargeMonsterConfigurtion.ChargeMonsterStages.Idle) {
                WaitForChangeDirection();
                animator.SetInteger("EnemyState", 1);
            }else if (currentStage == ChargeMonsterConfigurtion.ChargeMonsterStages.Dizzy) {
                WaitForDizzy();
                animator.SetInteger("EnemyState", 1);
            }
        }

        public override void OnDie() {
            base.OnDie();
            outlineSpriteRenderer = deathSpriteRenderer;
        }

        private void WaitForDizzy() {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0) {
                TriggerEvent(ChargeMonsterConfigurtion.ChargeMonsterEvents.WaitEnds);
                waitTime = startWaitTime;
            }
        }

        private void WaitForChangeDirection() {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0) {
                TriggerEvent(ChargeMonsterConfigurtion.ChargeMonsterEvents.WaitEnds);
                nextSpot = DecideDistance(transform.position);
                waitTime = startWaitTime;
            }
           
        }

        protected override void OnNotSeePlayer() {
            Debug.Log("Not see player");
            if (CurrentFSMStage == ChargeMonsterConfigurtion.ChargeMonsterStages.Chasing ||
                CurrentFSMStage == ChargeMonsterConfigurtion.ChargeMonsterStages.Attacking) {
                TriggerEvent(ChargeMonsterConfigurtion.ChargeMonsterEvents.PlayerOutChagseRange);
            }
            
        }

        protected override void OnSeePlayer() {
            if (CurrentFSMStage != ChargeMonsterConfigurtion.ChargeMonsterStages.Dizzy) {
                target = Player.Singleton.gameObject;
                TriggerEvent(ChargeMonsterConfigurtion.ChargeMonsterEvents.PlayerInChaseRange);
            }
           
        }

      

        private void Patrolling()
        {
            //Debug.Log("Patrolling");
            float direction = FaceLeft ? -1 : 1;
            rigidbody.velocity = direction * new Vector2(speed, rigidbody.velocity.y);

          //  transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextSpot.x, startLocation.y), speed * Time.deltaTime);

            FaceLeft = (nextSpot.x - transform.position.x) < 0;
           

            if (Vector2.Distance(transform.position, nextSpot) < 0.5f)
            {
                TriggerEvent(ChargeMonsterConfigurtion.ChargeMonsterEvents.WaitForChangeDirection);
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

        private void Chasing() {

            FaceLeft = (target.transform.position.x - transform.position.x) <= 0;
            float direction = FaceLeft ? -1 : 1;
            rigidbody.velocity = direction * new Vector2(chaseSpeed, rigidbody.velocity.y);
            //transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.transform.position.x, startLocation.y), chaseSpeed * Time.deltaTime);
          
        }

     

        private void Update()
        {
            base.Update();
            
            if (attackTrigger2DCheck.Triggered) {
                if (CurrentFSMStage != ChargeMonsterConfigurtion.ChargeMonsterStages.Dizzy) {
                    target = Player.Singleton.gameObject;
                    TriggerEvent(ChargeMonsterConfigurtion.ChargeMonsterEvents.PlayerInAttackRange);
                }
              
            }

            if (Attackable.Health.Value > 0) {
                outlineSpriteRenderer = aliveSpriteRenderer;
                outlineSpriteRenderer.sprite = spriteRenderer.sprite;
            }

            transform.localScale = FaceLeft? new Vector3(1,1,1) : new Vector3(-1,1,1);

            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0) {
                spriteRenderer.color = Color.white;
            }
        }


      

        protected override void OnAttackingStage(Enum attackStage) {
            float direction = FaceLeft ? 1 : -1;
            waitTime = dizzyTime;
            this.GetSystem<IAbsorbSystem>().AbsorbInterrupt();
            rigidbody.AddForce(new Vector2(direction * 35,20), ForceMode2D.Impulse);
            HurtPlayerWithCurrentAttackStage();
            this.SendCommand<TimeSlowCommand>(TimeSlowCommand.Allocate(0.5f,0.2f));
            Debug.Log("Hurt");
            Player.Singleton.GetComponent<Rigidbody2D>().AddForce(new Vector2(-direction *15, 8), ForceMode2D.Impulse);
            TriggerEvent(ChargeMonsterConfigurtion.ChargeMonsterEvents.AttackDizzy);
        }

        private float hurtTimer = 0.3f;
        public override void OnAttacked(float damage) {
            base.OnAttacked(damage);
            if (Attackable.Health.Value > 0) {
                //not die
                //idle
                if (damage >= 2) {
                    animator.SetTrigger("Hurt");
                }
                else {
                    spriteRenderer.color = Color.red;
                    hurtTimer = 0.3f;
                }
               
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector2(min_X, startLocation.y), new Vector2(max_X, startLocation.y));
        }
    }
}