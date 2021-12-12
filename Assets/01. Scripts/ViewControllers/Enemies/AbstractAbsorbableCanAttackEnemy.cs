using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class AbstractAbsorbableCanAttackEnemy<T, AttackStageEnum> : AbstractAbsorbableEnemy<T>,
        IEnemyViewControllerCanAttack<AttackStageEnum> where T : EnemyConfigurationItem, new()
    where AttackStageEnum : Enum
    {
        [SerializeField]
        protected Transform eyePosition;

        [SerializeField]
        protected float viewDistance = 10;

        [SerializeField]
        protected LayerMask eyeDetectLayers;

        [SerializeField]
        protected Transform bulletSpawnPosition;

        [SerializeField] protected GameObject bulletPrefab;

        protected bool FaceLeft
        {
            get
            {
                return faceLeft;
            }
            set
            {
                faceLeft = value;
            }
        }

        private bool faceLeft;
        public AttackStageEnum CurrentFSMStage
        {
            get
            {
                return (AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name, true);
            }
        }
        public ICanAttack CanAttackConfig
        {
            get
            {
                return (configurationItem) as ICanAttack;
            }
        }

        public bool IsAttacking
        {
            get
            {
                foreach (Enum attackStageName in CanAttackConfig.AttackStageNames)
                {
                    if (attackStageName.ToString() == FSM.CurrentState.name)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        protected void Update()
        {
            base.Update();

            foreach (Enum attackStageName in CanAttackConfig.AttackStageNames)
            {
                if (attackStageName.ToString() == FSM.CurrentState.name)
                {
                    OnAttackingStage(attackStageName);
                    break;
                }
            }

            float direction = FaceLeft ? -1 : 1;

            if (eyePosition)
            {
                RaycastHit2D hit = Physics2D.Raycast(eyePosition.position, eyePosition.right * direction, viewDistance,
                    eyeDetectLayers, -50f, 50f);

                if (hit.collider)
                {
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        if (Player.Singleton.CurrentState != PlayerState.Die)
                        {
                            OnSeePlayer();
                        }
                        else
                        {
                            OnNotSeePlayer();
                        }
                    }
                    else
                    {
                        OnNotSeePlayer();
                    }
                }
                else
                {
                    OnNotSeePlayer();
                }
            }


            OnFSMStage(CurrentFSMStage);
        }


        protected abstract void OnSeePlayer();

        protected void TriggerEvent(Enum eventEnum)
        {
            FSM.HandleEvent(eventEnum);
        }

        /// <summary>
        /// Damage = damage of current attack stage. Should only be called when in attack stage
        /// </summary>
        public void HurtPlayerWithCurrentAttackStage()
        {
            if (IsAttacking)
            {
                this.GetModel<IPlayerModel>().ChangeHealth(-CanAttackConfig.AttackSkillDamages
                    [(AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name)]);
            }
        }

        public float GetCurrentAttackRate()
        {
            if (IsAttacking)
            {
                return (CanAttackConfig.AttackFreqs
                    [(AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name)]);
            }

            return 0;
        }

        public void HurtPlayerNoMatterWhatAttackStage(float damage)
        {
            this.GetModel<IPlayerModel>().ChangeHealth(-damage);
        }

        private Tween moveTween;

        public override void OnAbsorbInterrupt()
        {
            StopAllCoroutines();
            Debug.Log("Mouse Interrupt");
            if (moveTween != null)
            {
                moveTween.Kill();
            }
            spriteRenderer.gameObject.transform.DOLocalMoveY(0, 0.3f);
        }

        public override void OnStartPrepareAbsorb()
        {
            base.OnStartPrepareAbsorb();
            StartCoroutine(Float());
        }


        public override void OnAbsorbed()
        {
            base.OnAbsorbed();
            if (moveTween != null)
            {
                moveTween.Kill();
            }
        }


        private IEnumerator Float()
        {
            yield return new WaitForSeconds(0.83f);

            spriteRenderer.transform.parent.DOShakePosition(1.67f, 0.2f, 20, 100);

            moveTween = spriteRenderer.gameObject.transform.DOMoveY(spriteRenderer.gameObject.transform.position.y + 3, 1.67f);
        }


        public abstract void OnAttackingStage(Enum attackStage);

        public abstract void OnFSMStage(AttackStageEnum currentStage);
        protected override void OnFSMStateChanged(string prevEvent, string newEvent)
        {

        }

        protected abstract void OnNotSeePlayer();
    }
}
