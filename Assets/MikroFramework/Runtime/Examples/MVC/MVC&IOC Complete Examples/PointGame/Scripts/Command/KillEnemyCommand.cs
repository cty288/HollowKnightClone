using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Examples;
using UnityEngine;

namespace MikroFramework.Examples
{
    public class KillEnemyCommand : AbstractCommand<KillEnemyCommand> {

        protected override void OnExecute() {
           this.GetModel<IGameModel>().KillCount.Value++;

           if (Random.Range(0, 10) < 3) {
               this.GetModel<IGameModel>().Gold.Value += Random.Range(1, 3);
           }

           this.SendEvent<OnKillEvent>();
            if (this.GetModel<IGameModel>().KillCount.Value == 10)
            {
                this.SendEvent<GameWInEvent>();
            }
        }
    }
}
