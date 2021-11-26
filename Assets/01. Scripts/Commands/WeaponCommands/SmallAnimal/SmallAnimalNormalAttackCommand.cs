using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HollowKnight
{
    public class SmallAnimalNormalAttackCommand : AbstractWeaponCommand<SmallAnimalNormalAttackCommand>
    {
        protected override void OnExecute() {
            WeaponInfo.BulletCount.Value--;
            Debug.Log("Small Animal attack");
            TargetAttackableViewController.AttackedByPlayer(WeaponInfo.AttackDamage.Value);
        }
    }
}
