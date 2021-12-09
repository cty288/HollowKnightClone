using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Examples.ServiceLocator;
using UnityEngine;
using FSM = MikroFramework.FSM.FSM;

namespace HollowKnight {
    public interface IEnemyViewController {
        public bool Attackable { get; }
        public bool Absorbable { get; }
        public bool CanAttack { get; }

        public bool CanAbsorb { get; }
    }

    public interface IEnemyViewControllerAbsorbable : IEnemyViewControllerAttackable {
        WeaponInfo WeaponInfo { get; }
        /// <summary>
        /// This is only effective if this enemy is absorbable in the configuration model
        /// </summary>
        void OnAbsorbing(float percentage);

        /// <summary>
        /// This is only effective if this enemy is absorbable in the configuration model
        /// </summary>
        void OnAbsorbed();

        /// <summary>
        /// When dropped after absorbed
        /// </summary>
        void OnDropped();
    }

    public interface IEnemyViewControllerAttackable {
        public IAttackable Attackable { get; }
        bool IsDie { get; }

        void AttackedByPlayer(int damage);

        /// <summary>
        /// This is only effect if the enemy is attackable
        /// </summary>
        void OnDie();

        GameObject GameObject { get; }
    }

    public interface IEnemyViewControllerCanAttack<AttackStageEnum> where AttackStageEnum:Enum{
         AttackStageEnum CurrentFSMStage { get; } 
         ICanAttack CanAttackConfig { get; }

        bool IsAttacking { get; }

        void HurtPlayerWithCurrentAttackStage();

        float GetCurrentAttackRate();

        void HurtPlayerNoMatterWhatAttackStage(float damage);

        void OnAttackingStage(Enum attackStage);

        void OnFSMStage(AttackStageEnum currentStage);
    }


    public abstract class EnemyBaseViewController<T> : AbstractMikroController<HollowKnight>, IEnemyViewController
        where T : EnemyConfigurationItem, new() {

        protected FSM FSM {
            get {
                return configurationItem.FSM;
            }
        }

        protected T configurationItem;

        protected IEnemyConfigurationModel enemyConfigurationModel;
        [SerializeField]
        protected Transform shakeParent;

        public bool Attackable {
            get {
                return typeof(T).GetInterface("IAttackable") != null; 
                
            }
        }

        public bool Absorbable {
            get { return typeof(T).GetInterface("IAbsorbable") != null; }
        }

        public bool CanAttack {
            get { return typeof(T).GetInterface("ICanAttack") != null; }
        }
        protected bool deathReady = true;
        public bool CanAbsorb {
            get {
                if (!Absorbable) return false;
                IAbsorbable absorbable = configurationItem as IAbsorbable;
                if (absorbable.Health.Value > 0) {
                    if (!absorbable.CanAbsorbWhenAlive) {
                        return false;
                    }
                }

                if (absorbable.Absorbed.Value) {
                    return false;
                }

                if (!deathReady) {
                    return false;
                }

                return true;
            }
        }

        protected virtual void Awake() {
            enemyConfigurationModel = this.GetModel<IEnemyConfigurationModel>();
            configurationItem = GetConfigurationItem();
            configurationItem.FSM.OnStateChanged.Register(OnFSMStateChanged);
        }

      
        

        protected virtual void OnDestroy() {
            configurationItem.FSM.OnStateChanged.UnRegister(OnFSMStateChanged);
        }

        protected StateEnum GetCurrentState<StateEnum>() where StateEnum : Enum {
            return (StateEnum) Enum.Parse(typeof(StateEnum), configurationItem.FSM.CurrentState.name);
        }

        protected abstract void OnFSMStateChanged(string prevEvent, string newEvent);

        protected T GetConfigurationItem() {

            return enemyConfigurationModel.GetEnemyConfigurationItemByType<T>();
        }

        protected void TriggerFSM<T>(T eventEnum) where T : Enum {
            configurationItem.FSM.HandleEvent(eventEnum);
        }
        
    }
    
    



    
}

