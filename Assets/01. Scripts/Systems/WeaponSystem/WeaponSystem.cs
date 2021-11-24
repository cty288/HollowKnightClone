using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace HollowKnight
{
    public struct OnWeaponShifted {
        public bool Up;
    }

    public interface IWeaponSystem : ISystem {
        WeaponInfo SelectedWeapon { get; }
        List<WeaponInfo> EquippedWeapons { get; }
        WeaponInfo GetWeaponFromConfig(WeaponName weaponName);
        int WeaponCount { get; }

        int BackpackCapacity { get; }
        void ShiftWeapon(bool up);
    }

    public class WeaponSystem : AbstractSystem, IWeaponSystem {

        public List<WeaponInfo> weaponList;

        private int selectIndex = 0;
        private IWeaponConfigModel configModel;

       

        public WeaponInfo SelectedWeapon {
            get {
                if (weaponList.Count > 0) {
                    return weaponList[0];
                }

                return null;
            }
        }

        public List<WeaponInfo> EquippedWeapons
        {
            get {
                return weaponList;
            }
        }
        public int WeaponCount {
            get {
                return weaponList.Count;
            }
        }
        public int BackpackCapacity { get; } = 4;


        protected override void OnInit() {
            configModel = this.GetModel<IWeaponConfigModel>();

            weaponList = new List<WeaponInfo>();

            this.RegisterEvent<OnWeaponAddedToBackpack>(OnWeaponAdded);
        }

        private void OnWeaponAdded(OnWeaponAddedToBackpack e) {
            AddWeaponToBackpack(e.WeaponInfo);
        }

        /// <summary>
        /// Return the dropped weapon (if the backpack is full)
        /// </summary>
        /// <returns></returns>
        public WeaponInfo AddWeaponToBackpack(WeaponInfo weaponInfo) {
            WeaponInfo droppedWeapon = null;
            if (weaponList.Count >= BackpackCapacity) {
                //drop one weapon
                droppedWeapon = weaponList[0];
                weaponList.RemoveAt(0);
            }

            weaponList.Insert(0, weaponInfo);
            Debug.Log($"Added {weaponInfo.Name.Value}, With Capacity {weaponInfo.BulletCount.Value}");
            return droppedWeapon;
        }

        public void ShiftWeapon(bool up) {
            if (WeaponCount > 1) {
                int lastIndex = WeaponCount - 1;
                WeaponInfo first = weaponList[0];
                WeaponInfo last = weaponList[lastIndex];

                if (up) {
                    weaponList.RemoveAt(lastIndex);
                    weaponList.Insert(0, last);
                }
                else {
                    weaponList.RemoveAt(0);
                    weaponList.Add(first);
                }

                this.SendEvent<OnWeaponShifted>(new OnWeaponShifted() {Up = up});
                Debug.Log($"Weapon Shifted. Current Weapon: {SelectedWeapon.Name}");
            }
        }

        public WeaponInfo GetWeaponFromConfig(WeaponName weaponName) {
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

        
        
        
    }
}
