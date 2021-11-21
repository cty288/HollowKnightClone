using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using UnityEngine;

namespace HollowKnight
{
    public interface IWeaponSystem : ISystem {
        WeaponInfo SelectedWeapon { get; }
        List<WeaponInfo> EquippedWeapons { get; }
        void ShiftWeapon(bool up);
    }

    public class WeaponSystem : AbstractSystem, IWeaponSystem {
        public List<WeaponInfo> equippedWeapons;

        private int selectIndex = 0;
        private IWeaponConfigModel configModel;
        public WeaponInfo SelectedWeapon {
            get {
                if (equippedWeapons.Count > 0) {
                    return equippedWeapons[selectIndex];
                }

                return null;
            }
        }
        public List<WeaponInfo> EquippedWeapons
        {
            get
            {
                return equippedWeapons;
            }
        }
        protected override void OnInit() {
            configModel = this.GetModel<IWeaponConfigModel>();

            equippedWeapons = new List<WeaponInfo>() {
                GetWeaponFromConfig(WeaponName.Chicken)
            };
        }

        private WeaponInfo GetWeaponFromConfig(WeaponName weaponName) {
            WeaponConfigItem configItem = configModel.GetWeaponByName(weaponName);
            WeaponInfo weaponInfo = new WeaponInfo(configItem.WeaponName,
                configItem.TypeConfigItem.Type, configItem.TypeConfigItem.AttackSkill,
                configItem.TypeConfigItem.AttackDamage, configItem.TypeConfigItem.AttackFreq,
                configItem.TypeConfigItem.ChargeAttackTime, configItem.TypeConfigItem.ChargeAttackDamage,
                configItem.TypeConfigItem.Ult, configItem.TypeConfigItem.UltChargeTime,
                configItem.TypeConfigItem.UltDamage, configItem.WeaponCapacity);
            return weaponInfo;
        }

        private WeaponInfo GetWeaponFromConfig(WeaponName weaponName, int bulletInGun)
        {
            WeaponConfigItem configItem = configModel.GetWeaponByName(weaponName);
            WeaponInfo weaponInfo = new WeaponInfo(configItem.WeaponName,
                configItem.TypeConfigItem.Type, configItem.TypeConfigItem.AttackSkill,
                configItem.TypeConfigItem.AttackDamage, configItem.TypeConfigItem.AttackFreq,
                configItem.TypeConfigItem.ChargeAttackTime, configItem.TypeConfigItem.ChargeAttackDamage,
                configItem.TypeConfigItem.Ult, configItem.TypeConfigItem.UltChargeTime,
                configItem.TypeConfigItem.UltDamage, bulletInGun);
            return weaponInfo;
        }

        public void ShiftWeapon(bool up) {
            
        }
    }
}
