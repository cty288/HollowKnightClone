using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public struct OnHumanoidBulletConsumed
    {
        public Vector2 TargetPos;
        public int ConsumeNumber;
        public bool ShootInstant;
        public WeaponInfo WeaponInfo;
    }
    public class HumanoidNormalAttackCommand : AbstractWeaponCommand<HumanoidNormalAttackCommand> { 
      
        protected override void OnExecute() {
            this.SendEvent<OnHumanoidBulletConsumed>(new OnHumanoidBulletConsumed()
            {
                TargetPos = this.TargetPosition,
                ConsumeNumber = 1,
                ShootInstant = true,
                WeaponInfo = WeaponInfo
            });
            //buff
            Debug.Log("OnHumanoidBulletConsumed");
            WeaponInfo.ConsumeBullet(1);
        }
    }
}
