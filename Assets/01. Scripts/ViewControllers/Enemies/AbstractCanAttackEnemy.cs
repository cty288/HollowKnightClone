using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

namespace HollowKnight
{
    public abstract class AbstractCanAttackEnemy<T, AttackStageEnum> : EnemyBaseViewController<T>,
         IEnemyViewControllerAttackable, IEnemyViewControllerCanAttack<AttackStageEnum>
         where T : EnemyConfigurationItem, new() where AttackStageEnum : Enum {

        protected Rigidbody2D rigidbody;

        protected IWeaponSystem weaponSystem;

        [SerializeField]
        protected SpriteRenderer outlineSpriteRenderer;
       
        public IAttackable Attackable
        {
            get
            {
                return (configurationItem) as IAttackable;
            }
        }

        protected virtual void Update()
        {
            
            foreach (Enum attackStageName in CanAttackConfig.AttackStageNames)
            {
                if (attackStageName.ToString() == FSM.CurrentState.name)
                {
                    OnAttackingStage(attackStageName);
                    break;
                }
            }
            
            OnFSMStage(CurrentFSMStage);
        }

        protected void TriggerEvent(Enum eventEnum)
        {
            FSM.HandleEvent(eventEnum);
        }

       
        protected override void Awake()
        {
            base.Awake();
            rigidbody = this.GetComponent<Rigidbody2D>();
        }

        protected virtual void Start()
        {

            this.RegisterEvent<OnAttackAiming>(OnAiming).UnRegisterWhenGameObjectDestroyed(gameObject);

            (configurationItem as IAttackable).Health.RegisterOnValueChaned(OnHealthChanged)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            //this.RegisterEvent<OnPlayerRespawned>(OnRespawned).UnRegisterWhenGameObjectDestroyed(gameObject);

            weaponSystem = this.GetSystem<IWeaponSystem>();

        }

        private void OnRespawned(OnPlayerRespawned obj) {
            if (!IsDie) {
                Attackable.Restore();
            }
        }

        private void OnHealthChanged(float old, float newHealth)
        {
            if (newHealth < old)
            {
                Debug.Log($"Attacked, add to charge {old - newHealth}");
                this.SendCommand<ChargeUltCommand>(ChargeUltCommand.Allocate(old - newHealth));
                OnAttacked(old - newHealth);
            }

            if (newHealth <= 0)
            {
                Debug.Log("Die");
                OnDie();
            }
        }

        public bool IsDie
        {
            get
            {
                return Attackable.Health.Value <= 0;
            }
        }

        public void AttackedByPlayer(int damage)
        {
            Attackable.Attack(damage);
        }

        public bool BornToBeDead { get; set; } = false;

        public virtual void OnDie() { }

        protected virtual void OnMouseOver()
        {

            IAbsorbSystem absorbSystem = this.GetSystem<IAbsorbSystem>();
            IAttackSystem attackSystem = this.GetSystem<IAttackSystem>();

            if (absorbSystem.AbsorbState != AbsorbState.NotAbsorbing)
            {
                if (absorbSystem.AbsorbingGameObject != this.gameObject)
                {
                    return;
                }
            }

            if (attackSystem.AttackState != AttackState.NotAttacking)
            {
                return;
            }

            outlineSpriteRenderer.enabled = true;
        }




        protected virtual void OnMouseExit()
        {
            if (this.GetSystem<IAbsorbSystem>().AbsorbState == AbsorbState.NotAbsorbing)
            {
                outlineSpriteRenderer.enabled = false;
            }

        }


        public GameObject GameObject
        {
            get
            {
                return this.gameObject;
            }
        }


        public AttackStageEnum CurrentFSMStage
        {
            get
            {
                return (AttackStageEnum)Enum.Parse(typeof(AttackStageEnum), FSM.CurrentState.name, true);
            }
        }

        private void OnAiming(OnAttackAiming e)
        {
            if (e.Target == this.gameObject)
            {
                outlineSpriteRenderer.enabled = true;
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

        public abstract void OnAttackingStage(Enum attackStage);

        public abstract void OnAttacked(float damage);

        public abstract void OnFSMStage(AttackStageEnum currentStage);

    }
}
