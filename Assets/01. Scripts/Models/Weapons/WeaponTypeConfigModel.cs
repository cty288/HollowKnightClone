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

            {WeaponType.SmallAnimal, new WeaponTypeConfigItem(WeaponType.SmallAnimal,null,10,0.2f,3,10,null,3,30)},

        };

        protected override void OnInit() {
            
        }

        public WeaponTypeConfigItem GetWeaponType(WeaponType name) {
            return weaponTypes[name];
        }
    }
}
