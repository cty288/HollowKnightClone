using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class SmallAnimalNormalAttackCommand : AbstractWeaponCommand<SmallAnimalNormalAttackCommand>
    {
        public struct OnSmallAnimalBulletConsumed {
            public IEnemyViewControllerAttackable TargetAttackable;
            public GameObject TargetGameObject;
            public int ConsumeNumber;
            public bool ShootInstant;
            public WeaponInfo WeaponInfo;
        }
        protected override void OnExecute() {
            this.SendEvent<OnSmallAnimalBulletConsumed>(new OnSmallAnimalBulletConsumed()
            {
                TargetGameObject = this.TargetGameObject,
                TargetAttackable = TargetAttackableViewController,
                ConsumeNumber = 1,
                ShootInstant = true,
                WeaponInfo = WeaponInfo
            });
            //buff
            WeaponInfo.ConsumeBullet(1);
           
            //TargetAttackableViewController.AttackedByPlayer(WeaponInfo.AttackDamage.Value);
        }
    }
}
