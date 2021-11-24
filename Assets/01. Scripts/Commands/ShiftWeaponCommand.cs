using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    
    public class ShiftWeaponCommand : AbstractCommand<ShiftWeaponCommand> {
        public bool Up;

        public static ShiftWeaponCommand Allocate(bool Up) {
            ShiftWeaponCommand cmd =  SafeObjectPool<ShiftWeaponCommand>.Singleton.Allocate();
            cmd.Up = Up;
            return cmd;
        }

        protected override void OnExecute() {
            this.GetSystem<IWeaponSystem>().ShiftWeapon(Up);
        }
    }
}
