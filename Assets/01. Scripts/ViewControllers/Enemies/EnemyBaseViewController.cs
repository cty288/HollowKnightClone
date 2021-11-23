using System;
using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public abstract class EnemyBaseViewController<T> : AbstractMikroController<HollowKnight>
    where T : EnemyConfigurationItem, new(){

        protected T configurationItem;

        protected IEnemyConfigurationModel enemyConfigurationModel;

        public bool Attackable {
            get {
                return typeof(T).GetInterface("IAttackable") != null;
            }
        }

        public bool Absorbable {
            get {
                return typeof(T).GetInterface("IAbsorbable") != null;
            }
        }

        public bool CanAttack {
            get {
                return typeof(T).GetInterface("ICanAttack") != null;
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

        protected StateEnum GetCurrentState<StateEnum>() where StateEnum:Enum {
            return (StateEnum) Enum.Parse(typeof(StateEnum), configurationItem.FSM.CurrentState.name);
        }

        protected abstract void OnFSMStateChanged(string prevEvent, string newEvent);

        protected T GetConfigurationItem() {
            
            return enemyConfigurationModel.GetEnemyConfigurationItemByType<T>();
        }

        protected void TriggerFSM<T>(T eventEnum) where T:Enum{
            configurationItem.FSM.HandleEvent(eventEnum);
        }

        public void Absorb() {
            if (Absorbable) {
                OnAbsorbed();
            }
        }


        public void AttackedByPlayer() {
            if (Attackable) {
                OnAttackedByPlayer();
            }
        }

        /// <summary>
        /// This is only effective if this enemy is absorbable in the configuration model
        /// </summary>
        protected virtual void OnAbsorbed() { }
        /// <summary>
        /// This is only effective if this enemy is attackable in the configuration model
        /// </summary>
        protected virtual void OnAttackedByPlayer() { }
    }
}
