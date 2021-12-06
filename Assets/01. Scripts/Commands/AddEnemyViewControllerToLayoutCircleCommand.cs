using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public struct OnAbsorbableEnemyViewControllerAddedToLayoutCircle
    {
        public SpriteRenderer targetSprite;
        public IEnemyViewControllerAbsorbable viewControllerAbsorbable;
        public GameObject viewControllerGameObject;
    }

    public struct OnWeaponAddedToBackpack {
        public WeaponInfo WeaponInfo;
    }

    public class AddEnemyViewControllerToLayoutCircleCommand : AbstractCommand<AddEnemyViewControllerToLayoutCircleCommand>
    {
        private IEnemyViewControllerAbsorbable viewControllerAbsorbable;
        private GameObject viewControllerGameObject;

        public static AddEnemyViewControllerToLayoutCircleCommand Allocate(IEnemyViewControllerAbsorbable viewControllerAbsorbable, GameObject viewControllerGameObject)
        {

            AddEnemyViewControllerToLayoutCircleCommand cmd =
                SafeObjectPool<AddEnemyViewControllerToLayoutCircleCommand>.Singleton.Allocate();

            cmd.viewControllerAbsorbable = viewControllerAbsorbable;
            cmd.viewControllerGameObject = viewControllerGameObject;
            return cmd;

        }

        protected override void OnExecute()
        {
            this.SendEvent<OnAbsorbableEnemyViewControllerAddedToLayoutCircle>(new OnAbsorbableEnemyViewControllerAddedToLayoutCircle()
            {
                viewControllerAbsorbable = viewControllerAbsorbable,
                viewControllerGameObject = viewControllerGameObject
            });

            this.SendEvent<OnWeaponAddedToBackpack>(new OnWeaponAddedToBackpack() {
                WeaponInfo = viewControllerAbsorbable.WeaponInfo
            });
        }
    }
}