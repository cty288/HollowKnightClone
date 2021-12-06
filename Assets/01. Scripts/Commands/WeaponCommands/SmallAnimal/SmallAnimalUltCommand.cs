using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class SmallAnimalUltCommand : AbstractUltCommand<SmallAnimalUltCommand> {
        protected override void OnUltExecuted() {
            this.GetSystem<IBuffSystem>().AddBuff(BuffType.SmallAnimalUnlimitedBullet,WeaponInfo.UltChargeTime.Value);
        }
    }
}
