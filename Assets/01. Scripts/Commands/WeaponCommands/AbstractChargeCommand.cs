using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public abstract class AbstractChargeCommand<T> : AbstractWeaponCommand<T>, IWeaponCommand where T : AbstractChargeCommand<T>, new() {

        private bool prevReleased = false;
        
        /// <summary>
        /// Is the "time" in the weapon info means a phrase?
        /// </summary>
        protected abstract bool TriggerByPhrase { get; }

        private ITimeSystem timeSystem;

        protected int phrase = 0;

        protected override void OnExecute() {
            if (timeSystem == null) {
                timeSystem = this.GetSystem<ITimeSystem>();
                Init();
            }

            if (!prevReleased && Released) {
                timeSystem.Pause();
                OnChargeReleased();
            }
            else {
                OnCharging();
            }

            prevReleased = Released;
        }

        void Init() {
            if (TriggerByPhrase) {
                timeSystem.AddDelayTask(WeaponInfo.ChargeAttackTime.Value, FinishPhrase);
            }
        }

        private void FinishPhrase() {
            if (!Released) {
                phrase++;
                OnOnePhaseFinished();
                if (WeaponInfo != null)
                {
                    timeSystem.AddDelayTask(WeaponInfo.ChargeAttackTime.Value, FinishPhrase);
                }
            }
           
           
        }

        /// <summary>
        /// Only triggered when this charge command has multiple phrase
        /// </summary>
        protected abstract void OnOnePhaseFinished();

        protected abstract void OnChargeReleased();

        protected abstract void OnCharging();

        /// <summary>
        /// Release current charge command manually. This will also trigger OnChargeReleased
        /// </summary>
        protected void ManualRelease() {
           
            timeSystem.Pause();
            OnChargeAttackRelease e = new OnChargeAttackRelease() { TotalChargeTime = Time };
            if (TargetAttackableViewController != null)
            {
                e.AttackableViewController = TargetAttackableViewController;
                e.TargetGameObject = TargetGameObject;
            }
            this.SendEvent<OnChargeAttackRelease>(e);

            this.GetSystem<IAttackSystem>().StopAttack();
        }
    }
}
