using System.Collections;
using System.Collections.Generic;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class HurtEnemyCommand : AbstractWeaponCommand<HurtEnemyCommand> {
        public IAttackable Attackable;
        public int Damage;

        public static HurtEnemyCommand Allocate(IAttackable attackable, int Damage) {
            HurtEnemyCommand cmd = SafeObjectPool<HurtEnemyCommand>.Singleton.Allocate();
            cmd.Attackable = attackable;
            cmd.Damage = Damage;
            return cmd;
        }

        protected override void OnExecute() {
            Attackable.Attack(Damage);
        }
    }
}
