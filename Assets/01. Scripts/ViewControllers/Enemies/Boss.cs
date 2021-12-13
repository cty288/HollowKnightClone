using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HollowKnight
{

    public struct OnPlayerEnterBossRoom {

    }

    public struct OnBossDie {

    }
    public class Boss : AbstractCanAttackEnemy<BossConfiguration, BossConfiguration.BossStages>, ICanSendEvent {
        /*                     internal state                     Animation State
        Attack                        0                                   0
        JumpAttack                    1                                   1
        LeftRightAttack               2                                   2
        Walk                          3                                   3
        Shockwave                     4                                   0
        */

        [SerializeField] private List<Trigger2DCheck> rangeChecks = new List<Trigger2DCheck>();
        [SerializeField] private Trigger2DCheck entryCheck;

        [SerializeField] private List<float> dizzyTimes = new List<float>();

        [SerializeField] private Trigger2DCheck normalAttackTrigger;
        [SerializeField] private Trigger2DCheck rightAttackTrigger;
        [SerializeField] private Trigger2DCheck playerHeadCheck;

        [SerializeField] private LayerMask wallCheckMasks;

        [SerializeField] private float jumpSpeed = 3f;

        [SerializeField] private List<GameObject> absorbableDeadBodyPrefab = new List<GameObject>();


        private List<BossConfiguration.BossStages> motionStages = new List<BossConfiguration.BossStages>() {
            BossConfiguration.BossStages.Attack,
            BossConfiguration.BossStages.JumpAttack,
            BossConfiguration.BossStages.LeftRightAttack,
           // BossConfiguration.BossStages.Shockwave,
            BossConfiguration.BossStages.Walk
        };

        private Animator animator;

        private Dictionary<Trigger2DCheck, List<BossConfiguration.BossStages> > rangeMainMotions;
        
        [SerializeField]
        private float dizzyTimer = 1;

        [SerializeField]
        private BossConfiguration.BossStages LastStage = BossConfiguration.BossStages.Dizzy;

        private bool playerEnterBossRoom = false;

        [SerializeField]
        private Collider2D mouseCheckTrigger;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

      
        private bool FaceLeft {
            get {
                return transform.localScale.x > 0;
            }
        }

        protected override void Awake() {
            base.Awake();
            this.RegisterEvent<OnCutsceneCameraFirstMoveComplete>(OnCutsceneCameraFirstMoveComplete)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<OnBossCutSceneComplete>(OnBossCutSceneComplete).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnBossCutSceneComplete(OnBossCutSceneComplete obj) {
            playerEnterBossRoom = true;
        }

        private void OnCutsceneCameraFirstMoveComplete(OnCutsceneCameraFirstMoveComplete e) {
            rigidbody.gravityScale = 5;
            rigidbody.velocity += (FaceLeft ? -1 : 1) * new Vector2(5, 0);
            this.TriggerEvent(BossConfiguration.BossEvents.JumpAttack);
        }

        protected override void Start() {
            base.Start();
            animator = GetComponent<Animator>();
            ConfigureRangeMotions();
            rigidbody.gravityScale = 0;
        }

        private void ConfigureRangeMotions() {
            rangeMainMotions = new Dictionary<Trigger2DCheck, List<BossConfiguration.BossStages>>() {
                {rangeChecks[0], new List<BossConfiguration.BossStages>() {
                    BossConfiguration.BossStages.Walk,
                    //BossConfiguration.BossStages.JumpAttack
                }}, {
                    rangeChecks[1],
                    new List<BossConfiguration.BossStages>()
                        {BossConfiguration.BossStages.LeftRightAttack, BossConfiguration.BossStages.Attack}
                }, {
                    rangeChecks[2],
                    new List<BossConfiguration.BossStages>() {
                        BossConfiguration.BossStages.Attack,
                        BossConfiguration.BossStages.LeftRightAttack, BossConfiguration.BossStages.JumpAttack
                    }
                }, {
                    rangeChecks[3],
                    new List<BossConfiguration.BossStages>() {
                        BossConfiguration.BossStages.Attack, BossConfiguration.BossStages.LeftRightAttack,
                        BossConfiguration.BossStages.JumpAttack
                       // BossConfiguration.BossStages.Shockwave
                    }
                }
            };
        }

        private bool onEnterBossRommTriggered = false;
        protected override void Update() {
            base.Update();
            CheckMouseHover();
            if (entryCheck.Triggered) {
                if (!playerEnterBossRoom && !onEnterBossRommTriggered) {
                    onEnterBossRommTriggered = true;
                    this.SendEvent<OnPlayerEnterBossRoom>();
                }
                //playerEnterBossRoom = true;
            }


            if (steppingOnPlayerHead)
            {
                if (!playerHeadCheck.Triggered)
                {
                    steppingOnPlayerHead = false;
                    gameObject.layer = LayerMask.NameToLayer("Enemy");
                }
            }

            hurtTimer -= Time.deltaTime;
            if (hurtTimer <= 0)
            {
                spriteRenderer.color = Color.white;
            }

            outlineSpriteRenderer.sprite = spriteRenderer.sprite;
        }
        public override void OnFSMStage(BossConfiguration.BossStages currentStage) {
            if (playerEnterBossRoom) {
                Debug.Log(currentStage);
                switch (currentStage) {
                    case BossConfiguration.BossStages.Dizzy:
                        DizzyCountDown();
                        break;
                    case BossConfiguration.BossStages.Attack:
                        Attack();
                        break;
                    case BossConfiguration.BossStages.JumpAttack:
                        JumpAttack();
                        break;
                    case BossConfiguration.BossStages.LeftRightAttack:
                        LeftRightAttack();
                        break;
                    case BossConfiguration.BossStages.Shockwave:
                        ShockWave();
                        break;
                    case BossConfiguration.BossStages.Walk:
                        Walk();
                        break;
                }
            }
            else {
                if (currentStage == BossConfiguration.BossStages.JumpAttack) {
                    JumpAttack();
                }
                else {
                    DizzyCountDown();
                }
               
            }
        }

        


        #region Shockwave
        [SerializeField] private GameObject shockwavePrefab;
        [SerializeField] private Transform shockwaveSpawnPos;
        private void ShockWave() {
            animator.SetInteger("Motion", 0);
        }

        private void SpawnShockWave(bool faceLeft, float speed=10) {
            ShockWave shockWave = Instantiate(shockwavePrefab, shockwaveSpawnPos.position, Quaternion.identity)
                .GetComponent<ShockWave>();
            shockWave.speed = speed;
            shockWave.FaceRight = !faceLeft;

        }
        #endregion

        [SerializeField] private float hitPlayerForce = 20;
        private void AddForceToPlayer() {
            Player.Singleton.GetComponent<Rigidbody2D>().AddForce(new Vector2((FaceLeft ? -1 : 1) * hitPlayerForce, 5), ForceMode2D.Impulse);
        }

        #region LeftRightAttack
        private int leftRightAttackTime = 0;

        [SerializeField] private GameObject stonePrefab;
        [SerializeField] private float stoneSpawnHeight = 13;
        [SerializeField] private int stoneSpawnNumPerTime = 8;
        [SerializeField] private int deadbodySpawnNumPerTime = 2;
        private void LeftRightAttack() {
            animator.SetInteger("Motion", 2);
        }

        public void OnNormalAttackBeforeLeftRightAttackAnimationDown() {
            if (normalAttackTrigger.Triggered) {
                AddForceToPlayer();
                HurtPlayerWithCurrentAttackStage();
                
            }
            this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.3f, 0.5f, 20, 90));
            SpawnStoneAndDeadBody();
        }

        [SerializeField] private Vector2 spawnStoneXRange;
        private void SpawnStoneAndDeadBody() {
            float x1 = spawnStoneXRange.x;
            float x2 = spawnStoneXRange.y;

            for (int i = 0; i < stoneSpawnNumPerTime; i++) {
                float pos = Random.Range(x1, x2);
                GameObject stone =  Instantiate(stonePrefab, new Vector3(pos, stoneSpawnHeight, 0),
                    Quaternion.identity);
                stone.GetComponent<Rigidbody2D>().gravityScale = Random.Range(1f, 2.5f);
            }


            for (int i = 0; i < deadbodySpawnNumPerTime; i++) {
                int spawn = Random.Range(0, 2);

                if (spawn == 0) {
                    float pos = Random.Range(x1, x2);
                    GameObject prefab = absorbableDeadBodyPrefab[Random.Range(0, absorbableDeadBodyPrefab.Count)];

                    GameObject body = Instantiate(prefab, new Vector3(pos, stoneSpawnHeight + 5, 0),
                        Quaternion.identity);

                    body.GetComponent<IEnemyViewControllerAbsorbable>().BornToBeDead = true;

                    body.GetComponent<Rigidbody2D>().gravityScale = Random.Range(3f, 5f);
                }
               
            }
            //float x2 = 
        }

        public void OnLeftAttackAnimationDown()
        {
            if (normalAttackTrigger.Triggered)
            {
                AddForceToPlayer();
                HurtPlayerWithCurrentAttackStage();
               
            }
            this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.3f, 0.3f, 20, 90));
            SpawnStoneAndDeadBody();
        }

        public void OnRightAttackAnimationDown() {
            if (rightAttackTrigger.Triggered)
            {
                HurtPlayerWithCurrentAttackStage();
                AddForceToPlayer();

            }
            this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.3f, 0.3f, 20, 90));
            SpawnStoneAndDeadBody();
        }

        public void OnLeftRightOneCycleFinished() {
            leftRightAttackTime--;
            if (leftRightAttackTime <= 0) {
                animator.SetTrigger("Dizzy");
                ToDizzyStage(2);
            }
        }

        #endregion

        #region Jump Attack
        enum BossJumpState {
            Prepare,
            Jumping,
            Landing
        }
        Vector2 targetjumpPos;
        private Vector2 startJumpPos;
        private BossJumpState jumpState = BossJumpState.Prepare;

        private bool steppingOnPlayerHead = false;
        private void JumpAttack()
        {
            //depend on player's location.
            //d1 d2: jump forward somewhere in d3
            //d3: to player's location
            //d4: somewhere in d3

            //check wall first
            float direction = FaceLeft ? -1 : 1;
            if (jumpState == BossJumpState.Prepare) {
                float d3ClosestX = GetRangeTriggerWorldPositionXVector2Range(rangeChecks[2]).x;
                Debug.Log($"D3 closestX: {d3ClosestX}");


                float distance = Mathf.Abs(d3ClosestX - transform.position.x);
               
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, distance, wallCheckMasks);

                if (hit.collider == null)
                { // can jump
                    

                    if (rangeChecks[2].Triggered) {
                        if (transform.position.x < Player.Singleton.transform.position.x) {
                            targetjumpPos = new Vector2(Player.Singleton.transform.position.x - 3.8f, transform.position.y);
                        }
                        else {
                            targetjumpPos = new Vector2(Player.Singleton.transform.position.x + 3.8f, transform.position.y);
                        }
                        
                    }
                    else {
                        float d3FarestX = GetRangeTriggerWorldPositionXVector2Range(rangeChecks[2]).y;
                        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, Vector2.right * direction, distance, wallCheckMasks);

                        float farBound = d3FarestX;

                        if (hit.collider)
                        {
                            farBound = hit.collider.gameObject.transform.position.x;
                        }

                        targetjumpPos = new Vector2(Random.Range(d3ClosestX, farBound), transform.position.y);
                    }

                    Debug.Log($"Target Jump Pos: {targetjumpPos.x}");

                    if (!playerEnterBossRoom) {
                        targetjumpPos = new Vector2(this.transform.position.x, Player.Singleton.transform.position.y);
                    }
                    //add force to rb
                    startJumpPos = transform.position;
                    animator.SetInteger("Motion", 1);
                }
                else { //cant jump because of wall -> turn
                    TurnBack();
                }

            }else if (jumpState == BossJumpState.Jumping) {
                gameObject.layer = LayerMask.NameToLayer("EnemyTraversable");
                float percentage = Mathf.Abs((startJumpPos.x - transform.position.x)) /
                                   Mathf.Abs(startJumpPos.x - targetjumpPos.x);

                animator.SetFloat("JumpAttackPercentage", percentage);


                /*
                if (Mathf.Abs(transform.position.x - targetjumpPos.x) <= 1) {
                    if (jumpTween != null) {
                        jumpTween.Kill();
                        rigidbody.AddForce(new Vector2(direction * 10,-200), ForceMode2D.Impulse);
                        animator.SetFloat("JumpAttackPercentage", 1);
                    }

                    jumpState = BossJumpState.Landing;
                }*/
            }else if (jumpState == BossJumpState.Landing) {
                Debug.Log("Landing");

                if (playerHeadCheck.Triggered) {
                    gameObject.layer = LayerMask.NameToLayer("EnemyTraversable");
                    steppingOnPlayerHead = true;
                }
                else {
                    gameObject.layer = LayerMask.NameToLayer("Enemy");
                }

                if (normalAttackTrigger.Triggered && !jumpAttackPlayerHurt) {
                    jumpAttackPlayerHurt = true;
                    HurtPlayerWithCurrentAttackStage();

                    AddForceToPlayer();
                }
            }
            
        }

        private bool jumpAttackPlayerHurt = false;
        private Tween jumpTween;
        public void OnJumpAnimationReady() {
            jumpState = BossJumpState.Jumping;
            float direction = FaceLeft ? -1 : 1;
            float duration = Mathf.Abs(targetjumpPos.x - transform.position.x) / jumpSpeed;
            //rigidbody.AddForce(new Vector2(jumpVelocity.x * direction, jumpVelocity.y), ForceMode2D.Impulse);
            jumpTween = transform.DOMove(new Vector3(targetjumpPos.x, transform.position.y + 6, 0), duration)
                .SetEase(Ease.OutQuad).OnComplete(() => {
                    /*
                    rigidbody.AddForce(new Vector2(direction * 10, -200), ForceMode2D.Impulse);
                    animator.SetFloat("JumpAttackPercentage", 1);
                    jumpState = BossJumpState.Landing;*/
                });
        }

        public void OnJumpAttackStartDown() {
            float direction = FaceLeft ? -1 : 1;
            rigidbody.AddForce(new Vector2(direction * 10, -100), ForceMode2D.Impulse);
            jumpState = BossJumpState.Landing;
        }


        public void OnJumpAttackDown() {
            
            this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.5f, 1f, 20, 90));
        }

        public void OnJumpAttackFinished() {
            animator.SetTrigger("Dizzy");
            ToDizzyStage(1);
        }

        [SerializeField] private LayerMask groundCheckLayers;
        private void OnCollisionEnter2D(Collision2D other) {
            if (PhysicsUtility.IsInLayerMask(other.collider .gameObject, groundCheckLayers)) {
                if (jumpTween != null) {
                    jumpTween.Kill(true);
                }

                Debug.Log("Boss hit ground");

                if (jumpState == BossJumpState.Landing && (!playerEnterBossRoom || CurrentFSMStage == BossConfiguration.BossStages.JumpAttack)) {
                    Debug.Log("Jump attack hit ground");
                    animator.SetTrigger("JumpAttackHitGround");
                }
            }
        }
        #endregion

        #region Walk
        //depend on player's location and boss' location
        //player:
        //d1, d2: Turn, move back a small distance
        //d3, d4: Move Forward,
        //  until: Player in d1/d2  or  distance reach  

        //boss:
        //Face wall:Always Turn, jump forward

        [SerializeField] private float walkSpeed = 5f;
        private Vector2 targetWalkDest = Vector2.zero;

        private WalkDecision walkDecision = WalkDecision.DecisionNotMadeYet;
        enum WalkDecision {
            DecisionNotMadeYet,
            PlayerInD1D2,
            PlayerInD3D4,
          
        }

        private Vector2 lastWalkPos;
        private float totalWalkDis=0;
        private void Walk() {
            float direction = FaceLeft ? -1 : 1;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, 3, wallCheckMasks);

            if (hit.collider != null) {
                //face wall -> turn and jump forward
                TurnBack();
                SwitchToMotion(BossConfiguration.BossStages.JumpAttack);
                return;
            }



            switch (walkDecision) {
                case WalkDecision.DecisionNotMadeYet:
                    if (rangeChecks[0].Triggered || rangeChecks[1].Triggered) {
                        walkDecision = WalkDecision.PlayerInD1D2;
                        TurnBack();
                        direction = FaceLeft ? -1 : 1;
                        targetWalkDest = new Vector2(transform.position.x + direction * Random.Range(3, 10),
                            transform.position.y);
                        animator.SetInteger("Motion", 3);
                        
                    }
                    else if (rangeChecks[2].Triggered || rangeChecks[3].Triggered) {
                        walkDecision = WalkDecision.PlayerInD3D4;
                        targetWalkDest = new Vector2(transform.position.x + direction * Random.Range(3, 10),
                            transform.position.y);
                        animator.SetInteger("Motion", 3);
                    }
                    else { //player on the back
                        TurnBack();
                    }
                    Debug.Log($"Target walk dest: {targetWalkDest.x}");

                    break;
                   
                case WalkDecision.PlayerInD1D2:
                    if (Vector2.Distance(transform.position, targetWalkDest) <= 1) {
                        animator.SetTrigger("Dizzy");
                        ToDizzyStage(3);
                        return;
                    }
                    break;
                case WalkDecision.PlayerInD3D4:
                    if (Vector2.Distance(transform.position, targetWalkDest) <= 1) {
                        animator.SetTrigger("Dizzy");
                        ToDizzyStage(3);
                        return;
                    }else if (rangeChecks[0].Triggered || rangeChecks[1].Triggered) {
                        animator.SetTrigger("Dizzy");
                        ToDizzyStage(3);
                        return;
                    } 
                    break;
            }

            totalWalkDis += Vector2.Distance(transform.position, lastWalkPos);
            lastWalkPos = transform.position;

            if (totalWalkDis >= 10) {
                animator.SetTrigger("Dizzy");
                ToDizzyStage(3);
                return;
            }

            direction = FaceLeft ? -1 : 1;
            rigidbody.velocity = new Vector2(direction * walkSpeed, 0);
        }
        #endregion
        
        #region Attack
        private void Attack() {
            animator.SetInteger("Motion", 0);
        }

        public void OnAnimationAttackDown() {
            if (normalAttackTrigger.Triggered) {
                Debug.Log("Attack hit player");
                HurtPlayerWithCurrentAttackStage();
                AddForceToPlayer();

            }
            this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.3f, 0.5f, 20, 90));

            int shockWaveChance = Random.Range(0, 100);
            if (shockWaveChance <= 50 || Attackable.Health.Value <= Attackable.MaxHealth/2
            || (!rangeChecks[0].Triggered && !rangeChecks[1].Triggered)) {
                SpawnShockWave(FaceLeft);
            }


            /*
            if (CurrentFSMStage == BossConfiguration.BossStages.Shockwave)
            {
                //spawn shock wave
                SpawnShockWave(FaceLeft);
                Debug.Log("Spawn shock wave!");
            }*/
        }

        public void OnAnimationAttackFinished() {
            Debug.Log("Attack finished");
            animator.SetTrigger("Dizzy");
            ToDizzyStage(0);
        }

        #endregion

        #region Dizzy
        private void ToDizzyStage(int index) {
            dizzyTimer = dizzyTimes[index];
            
            TriggerEvent(BossConfiguration.BossEvents.MovementFinish);
        }

        private bool stageSwitched = false;
        private void DizzyCountDown()
        {
            animator.SetInteger("Motion", -1);
            dizzyTimer -= Time.deltaTime;
            if (playerEnterBossRoom) {
                if (dizzyTimer <= 0 && !stageSwitched)
                {
                    stageSwitched = true;
                    StartCoroutine(SwitchStageDecision(bossStage => {
                        Debug.Log($"Decide to switch to {bossStage}");
                        //dizzyTimer = 1;
                        //animator.SetBool("Dizzy", false);
                        SwitchToMotion(bossStage);
                        stageSwitched = false;
                    }));
                }
            }
            
        }
        #endregion


        #region Motion Switch and Check
        private void SwitchToMotion(BossConfiguration.BossStages nextMotion)
        {
            
            switch (nextMotion) {
                    
                case BossConfiguration.BossStages.Attack:
                    LastStage = nextMotion;
                    TriggerEvent(BossConfiguration.BossEvents.Attack);
                    
                    break;
                case BossConfiguration.BossStages.JumpAttack:
                    LastStage = nextMotion;
                    jumpTween = null;
                    jumpAttackPlayerHurt = false;
                    animator.SetFloat("JumpAttackPercentage",0);
                    jumpState = BossJumpState.Prepare;
                    TriggerEvent(BossConfiguration.BossEvents.JumpAttack);
                    break;
                case BossConfiguration.BossStages.LeftRightAttack:
                    LastStage = nextMotion;
                    
                    leftRightAttackTime = Random.Range(3, 7);
                    TriggerEvent(BossConfiguration.BossEvents.LeftRightAttack);
                    break;
                case BossConfiguration.BossStages.Shockwave:
                    LastStage = nextMotion;
                    TriggerEvent(BossConfiguration.BossEvents.Shockwave);
                    break;
                case BossConfiguration.BossStages.Walk: //need some logics
                    LastStage = nextMotion;
                    totalWalkDis = 0;
                    lastWalkPos = transform.position;
                    walkDecision = WalkDecision.DecisionNotMadeYet;
                    TriggerEvent(BossConfiguration.BossEvents.MoveBack);
                    break;
            }
        }


        private IEnumerator SwitchStageDecision(Action<BossConfiguration.BossStages> onFinished) {
            Trigger2DCheck rangePlayerIn = null;

            while (true) {
                foreach (Trigger2DCheck trigger2DCheck in rangeChecks) {
                    if (trigger2DCheck.Triggered) {
                        rangePlayerIn = trigger2DCheck;
                        break;
                    }
                }

                if (rangePlayerIn != null) {
                    break;
                }
                else {
                    TurnBack();
                    yield return new WaitForSeconds(0.1f);
                }
            }

            if (CurrentFSMStage != BossConfiguration.BossStages.Die) {
                List<BossConfiguration.BossStages> currentRangeMainMotions = rangeMainMotions[rangePlayerIn];
                List<BossConfiguration.BossStages> currentRangeOtherMotions = new List<BossConfiguration.BossStages>();

                foreach (BossConfiguration.BossStages motionStage in motionStages)
                {
                    bool contains = false;
                    foreach (BossConfiguration.BossStages currentRangeMainMotion in currentRangeMainMotions)
                    {
                        if (currentRangeMainMotion == motionStage)
                        {
                            contains = true;
                        }
                    }

                    if (!contains)
                    {
                        currentRangeOtherMotions.Add(motionStage);
                    }
                }

                int motionType = Random.Range(0, 100);
                BossConfiguration.BossStages targetStage = BossConfiguration.BossStages.Attack;

                List<BossConfiguration.BossStages> targetMotionList = null;

                if (motionType <= 80 || currentRangeOtherMotions.Count == 0)
                {


                    targetMotionList = currentRangeMainMotions;

                }
                else
                {
                    targetMotionList = currentRangeOtherMotions;
                }


                do
                {
                    //main motions
                    if (targetMotionList == currentRangeMainMotions && targetMotionList.Contains(BossConfiguration.BossStages.JumpAttack))
                    {
                        int jumpChance = Random.Range(0, 11);
                        if (jumpChance >= 9)
                        {
                            targetStage = BossConfiguration.BossStages.JumpAttack;
                            break;
                        }
                    }

                    int targetMotionIndex = Random.Range(0, targetMotionList.Count);
                    targetStage = targetMotionList[targetMotionIndex];
                } while (targetStage == LastStage && targetMotionList.Count > 1);

                onFinished?.Invoke(targetStage);
            }
           
        }

        #endregion

        public void OnBossFallGround() {
            this.SendCommand<ShakeCameraCommand>(ShakeCameraCommand.Allocate(0.7f, 1.5f, 20, 100));
        }

        private Vector2 GetRangeTriggerWorldPositionXVector2Range(Trigger2DCheck range) {
            float localX = range.gameObject.transform.localPosition.x;
            float localScaleX = range.gameObject.transform.localScale.x;
            float rad = localScaleX / 2;

            float bound1 = localX + rad -4;
            float bound2 = localX - rad + 4;

            float worldBound1 = transform.TransformPoint(bound1, range.gameObject.transform.localPosition.y, 0).x;
            float worldBound2 = transform.TransformPoint(bound2, range.gameObject.transform.localPosition.y, 0).x;


           
            return new Vector2(worldBound1, worldBound2);
            
        }

        protected override void OnFSMStateChanged(string prevEvent, string newEvent) {
          
        }

        private void TurnBack() {
            transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
            foreach (Trigger2DCheck rangeCheck in rangeChecks) {
                rangeCheck.Clear();
            }
        }

        public override void OnAttackingStage(Enum attackStage) { }

        private float hurtTimer = 0.3f;

        public override void OnAttacked(float damage) {
            this.SendEvent<OnBossHurt>(new OnBossHurt()
                {currentHealth = Attackable.Health.Value, maxHealth = Attackable.MaxHealth});

            if (Attackable.Health.Value > 0) {
                spriteRenderer.color = Color.red;
                hurtTimer = 0.3f;
            }

        }

        [SerializeField] private Collider2D aliveCollider2D;
       // [SerializeField] private Collider2D diecCollider2D;

        public override void OnDie() {
            base.OnDie();
            animator.SetTrigger("Die");
            this.SendEvent<OnBossHurt>(new OnBossHurt()
                { currentHealth = 0, maxHealth = Attackable.MaxHealth });
            this.SendEvent<OnBossDie>(new OnBossDie());
            rigidbody.gravityScale = 5;
            //aliveCollider2D.isTrigger = true;
            mouseCheckTrigger.enabled = false;
            TriggerEvent(BossConfiguration.BossEvents.Killed);
        }

        protected override void OnMouseOver() { }

        protected override void OnMouseExit() { }

        private void CheckMouseHover()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider)
            {
                if (hit.collider is PolygonCollider2D || hit.collider == mouseCheckTrigger)
                {
                    OnMouseHover();
                    return;
                }
            }

            OnMouseLeave();
        }

        private void OnMouseLeave()
        {
            if (this.GetSystem<IAbsorbSystem>().AbsorbState == AbsorbState.NotAbsorbing)
            {
                outlineSpriteRenderer.enabled = false;
            }
        }

        private void OnMouseHover()
        {
            IAttackSystem attackSystem = this.GetSystem<IAttackSystem>();
            
            if (attackSystem.AttackState != AttackState.NotAttacking)
            {
                return;
            }

          
            outlineSpriteRenderer.enabled = true;
        }

    }

    public struct OnBossHurt{
        public float currentHealth;
        public float maxHealth;
    }
}
