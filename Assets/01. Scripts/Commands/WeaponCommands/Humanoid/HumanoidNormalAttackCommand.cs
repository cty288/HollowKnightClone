using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public struct OnHumanoidBulletConsumed
    {
        public IEnemyViewControllerAttackable TargetAttackable;
        public GameObject TargetGameObject;
        public int ConsumeNumber;
        public bool ShootInstant;
        public WeaponInfo WeaponInfo;
    }
    public class HumanoidNormalAttackCommand : AbstractWeaponCommand<SmallAnimalNormalAttackCommand> { 
      
        protected override void OnExecute() {
            this.SendEvent<OnHumanoidBulletConsumed>(new OnHumanoidBulletConsumed()
            {
                TargetGameObject = this.TargetGameObject,
                TargetAttackable = TargetAttackableViewController,
                ConsumeNumber = 1,
                ShootInstant = true,
                WeaponInfo = WeaponInfo
            });
            //buff
            WeaponInfo.ConsumeBullet(1);
        }
    }
}
