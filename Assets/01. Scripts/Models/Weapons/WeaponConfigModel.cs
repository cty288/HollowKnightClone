using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IWeaponConfigModel : IModel {
        WeaponConfigItem GetWeaponByName(WeaponName name);
        List<WeaponConfigItem> GetWeaponsByType(WeaponType type);
    }
    public class WeaponConfigModel : AbstractModel, IWeaponConfigModel {

        private Dictionary<WeaponName, WeaponConfigItem> weapons = new Dictionary<WeaponName, WeaponConfigItem>();
        private IWeaponTypeConfigModel weaponTypeConfig;

        protected override void OnInit() {
            weaponTypeConfig = this.GetModel<IWeaponTypeConfigModel>();
            RegisterWeapons();

            
        }

        private void RegisterWeapons() {
            RegisterWeapon(WeaponName.Rat, WeaponType.SmallAnimal, 3);
        }



        private void RegisterWeapon(WeaponName name, WeaponType type, int capacity) {
            weapons.Add(name,new WeaponConfigItem(name,weaponTypeConfig.GetWeaponType(type),capacity));
        }

        private void RegisterWeapon(WeaponName name, WeaponType type, int capacity,
            out WeaponTypeConfigItem overrideConfig) {
            WeaponTypeConfigItem originalTypeConfig = weaponTypeConfig.GetWeaponType(type);

            WeaponTypeConfigItem newTypeConfig = new WeaponTypeConfigItem(
                originalTypeConfig.Type, originalTypeConfig.AttackSkill,
                originalTypeConfig.AttackDamage, originalTypeConfig.AttackFreq,
                originalTypeConfig.ChargeAttackSkill,
                originalTypeConfig.ChargeAttackTime, originalTypeConfig.ChargeAttackDamage,
                originalTypeConfig.Ult, originalTypeConfig.UltChargeTime, originalTypeConfig.UltDamage);

            overrideConfig = newTypeConfig;

            weapons.Add(name,new WeaponConfigItem(name,overrideConfig,capacity));
        }

        public WeaponConfigItem GetWeaponByName(WeaponName name) {
            return weapons[name];
        }

        public List<WeaponConfigItem> GetWeaponsByType(WeaponType type) {
            var enumerator =  weapons.GetEnumerator();
            List<WeaponConfigItem> configItems = new List<WeaponConfigItem>();

            while (enumerator.MoveNext()) {
                WeaponConfigItem current = enumerator.Current.Value;
                if (current.TypeConfigItem.Type == type) {
                    configItems.Add(current);
                }
            }

            return configItems;
        }
    }
}

