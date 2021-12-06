using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class HumanoidUltCommand : AbstractUltCommand<HumanoidUltCommand>
    {
        protected override void OnUltExecuted()
        {
            this.GetSystem<IBuffSystem>().AddBuff(BuffType.HumanoidNormalAttackFaster,
                WeaponInfo.UltChargeTime.Value);

        }
    }
}