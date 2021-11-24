using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class KillEnemyCommand : AbstractCommand<KillEnemyCommand>
    {
        private IAttackable enemy;
        public static KillEnemyCommand Allocate(IAttackable enemy)
        {
            KillEnemyCommand cmd = SafeObjectPool<KillEnemyCommand>.Singleton.Allocate();
            cmd.enemy = enemy;
            return cmd;
        }

        protected override void OnExecute()
        {
            enemy.Kill();
        }
    }
}