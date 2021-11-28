using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace HollowKnight
{
    public struct OnSmallAnimalChargeReleased {

    }

    public class SmallAnimalChargeAttackCommand : AbstractChargeCommand<SmallAnimalChargeAttackCommand> {

        protected override bool TriggerByPhrase { get; } = true;

        protected override void OnOnePhaseFinished() {
            if (WeaponInfo.BulletCount.Value > 0) {
                Debug.Log("One phrase finished");
                this.SendEvent<SmallAnimalNormalAttackCommand.OnSmallAnimalBulletConsumed>(new SmallAnimalNormalAttackCommand.OnSmallAnimalBulletConsumed()
                {
                    TargetGameObject = this.TargetGameObject,
                    TargetAttackable = TargetAttackableViewController,
                    ConsumeNumber = 1,
                    ShootInstant = false,
                    WeaponInfo = WeaponInfo
                });

                WeaponInfo.ConsumeBullet(1);

                

                if (WeaponInfo.BulletCount.Value <= 0)
                {
                    ManualRelease();
                }
            }
            
        }

        protected override void OnChargeReleased() {
            Debug.Log("Charge release");
            //wait animation
            this.GetSystem<ITimeSystem>().AddDelayTask(0.2f, () => {
                this.SendEvent<OnSmallAnimalChargeReleased>();
            });

        }

        protected override void OnCharging() {
            
        }
    }
}
