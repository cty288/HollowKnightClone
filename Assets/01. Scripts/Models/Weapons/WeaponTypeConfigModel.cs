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

            {WeaponType.SmallAnimal, new WeaponTypeConfigItem(WeaponType.SmallAnimal,new SmallAnimalNormalAttackCommand(),
                1,0.25f,new SmallAnimalChargeAttackCommand(),0.5f,1.25f,new SmallAnimalUltCommand(),10,30,false)},

        };

        protected override void OnInit() {
            
        }

        public WeaponTypeConfigItem GetWeaponType(WeaponType name) {
            return weaponTypes[name];
        }
    }
}
