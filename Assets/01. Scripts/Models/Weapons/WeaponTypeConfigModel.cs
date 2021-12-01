using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace HollowKnight
{
    public interface IWeaponTypeConfigModel : IModel {
        WeaponTypeConfigItem GetWeaponType(WeaponType name);
    }


    public class WeaponTypeConfigModel : AbstractModel, IWeaponTypeConfigModel {
        private Dictionary<WeaponType, WeaponTypeConfigItem> weaponTypes = new Dictionary<WeaponType, WeaponTypeConfigItem>() {

            {WeaponType.SmallAnimal, new WeaponTypeConfigItem(WeaponType.SmallAnimal,
                new SmallAnimalNormalAttackCommand(),
                1,0.25f,
                new SmallAnimalChargeAttackCommand(),
                0.5f,1.25f,
                new SmallAnimalUltCommand(),10,0,false)},


            {WeaponType.Humanoid, new WeaponTypeConfigItem(WeaponType.Humanoid,
                new SmallAnimalNormalAttackCommand(),
                2,0.5f,
                new SmallAnimalChargeAttackCommand(),
                0.5f,2.5f,
                new SmallAnimalUltCommand(),10,0,false)},
        };

        protected override void OnInit() {
            
        }

        public WeaponTypeConfigItem GetWeaponType(WeaponType name) {
            return weaponTypes[name];
        }
    }
}
