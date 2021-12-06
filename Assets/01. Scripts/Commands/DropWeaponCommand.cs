using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public class DropWeaponCommand : AbstractCommand<DropWeaponCommand> {
        protected override void OnExecute() {
            this.GetSystem<IWeaponSystem>().DropSelectedWeapon();
        }
    }
}
