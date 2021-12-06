using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IWeaponCommand : ICommand {
        //input when configuring
        public IEnemyViewControllerAttackable TargetAttackableViewController { get; set; }
        public GameObject TargetGameObject { get; set; }
        public float Time { get; set; }

        public bool Released { get; set; }
        public Vector2 TargetPosition { get; set; }
        public WeaponInfo WeaponInfo { get; set; }
        //

        public IWeaponCommand Clone();

        
    }
}
