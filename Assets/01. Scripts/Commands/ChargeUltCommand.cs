using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class ChargeUltCommand : AbstractCommand<ChargeUltCommand> {
        public int ChargeAmount;

        public static ChargeUltCommand Allocate(int amount) {
            ChargeUltCommand cmd = SafeObjectPool<ChargeUltCommand>.Singleton.Allocate();
            cmd.ChargeAmount = amount;
            return cmd;
        }
        protected override void OnExecute() {
            Debug.Log($"ult added: {ChargeAmount}");
            this.GetModel<IPlayerModel>().AddUlt(ChargeAmount);
        }
    }
}
